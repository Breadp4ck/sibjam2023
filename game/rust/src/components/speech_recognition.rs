use std::fs::File;
use std::io::BufWriter;
use std::process::Command;
use std::sync::mpsc::{channel, Receiver, Sender};
use std::sync::{Arc, Mutex};

use cpal::traits::{DeviceTrait, HostTrait, StreamTrait};
use cpal::{FromSample, Sample};
use godot::engine::Os;
use godot::prelude::*;

/// Cpal stream config with parameters for PocketSphinx purposes
static CPAL_STREAM_CONFIG: cpal::StreamConfig = cpal::StreamConfig {
    channels: 1,
    sample_rate: cpal::SampleRate(16000),
    buffer_size: cpal::BufferSize::Default,
};

/// Hound wav file config for PocketSphinx purposes
static HOUND_WAV_SPEC: hound::WavSpec = hound::WavSpec {
    channels: 1,
    sample_rate: 16000,
    bits_per_sample: (std::mem::size_of::<i16>() * 8) as _, // cpal::SampleFormat::I16.sample_size() is not static
    sample_format: hound::SampleFormat::Int,
};

#[derive(GodotClass)]
#[class(base=Node)]
struct SpeechRecognitionComponent {
    /// Path to PocketSphinx executable file. Beware different OSs!
    #[export]
    windows_pocketsphinx_path: GString,

    #[export]
    linux_pocketsphinx_path: GString,

    /// Path to PocketSphinx acoustic model file (-hmm).
    #[export]
    acoustic_model_path: GString,

    /// Path to PocketSphinx language model file (-lm).
    #[export]
    language_model_path: GString,

    /// Path to PocketSphinx dictionary file (-dict).
    #[export]
    dictionary_path: GString,

    /// Path to file recorded to. Then it will be used by PocketSphinx.
    #[export]
    filename_to_record: GString,

    _host: cpal::Host,
    device: cpal::Device,
    stream: Option<cpal::Stream>,
    writer: Option<WavWriterHandle>,
    command_sender: Sender<PocketSphinxCmdData>,
    tokens_receiver: Receiver<Tokens>,
    pocketsphinx_path: String,
    parsing: bool,

    #[base]
    base: Base<Node>,
}

/// PocketSphinx is used as cmd app.
struct PocketSphinxCmdData {
    pocketsphinx_path: String,
    acoustic_model_path: String,
    language_model_path: String,
    dictionary_path: String,
    filename_path: String,
}

type Tokens = Vec<String>;

macro_rules! generate_input_stream {
    ($self:ident, $config:expr, $data:ident, $writer:expr, $err_fn:expr, $sample_format:ty) => {
        $self
            .device
            .build_input_stream(
                &$config,
                move |$data, _: &_| write_input_data::<$sample_format, i16>($data, &$writer),
                $err_fn,
                None,
            )
            .unwrap()
    };
}
#[godot_api]
impl SpeechRecognitionComponent {
    /// Start record process. If the record process or parsing process are already begun, then
    /// the function just return and will not do anything.
    #[func]
    pub fn start_record(&mut self) {
        // We don't want to begin new process, if prevoius is already has been started
        if self.parsing {
            return;
        }

        let writer =
            hound::WavWriter::create(self.filename_to_record.to_string(), HOUND_WAV_SPEC).unwrap();
        let writer = Arc::new(Mutex::new(Some(writer)));

        let err_fn = move |err| {
            godot_warn!("an error occurred on stream: {}", err);
        };

        godot_print!("Begin recording...");

        // Run the input stream on a separate thread.
        let writer_2 = writer.clone();

        let cfg = self
            .device
            .default_input_config()
            .expect("Error retieving input config");

        let stream = match cfg.sample_format() {
            cpal::SampleFormat::I8 => {
                generate_input_stream!(self, cfg.into(), data, writer_2, err_fn, i8)
            }
            cpal::SampleFormat::I16 => {
                generate_input_stream!(self, cfg.into(), data, writer_2, err_fn, i16)
            }
            cpal::SampleFormat::I32 => {
                generate_input_stream!(self, cfg.into(), data, writer_2, err_fn, i32)
            }
            cpal::SampleFormat::U8 => {
                generate_input_stream!(self, cfg.into(), data, writer_2, err_fn, u8)
            }
            cpal::SampleFormat::U16 => {
                generate_input_stream!(self, cfg.into(), data, writer_2, err_fn, u16)
            }
            cpal::SampleFormat::U32 => {
                generate_input_stream!(self, cfg.into(), data, writer_2, err_fn, u32)
            }
            cpal::SampleFormat::F32 => {
                generate_input_stream!(self, cfg.into(), data, writer_2, err_fn, f32)
            }
            cpal::SampleFormat::I64 => {
                generate_input_stream!(self, cfg.into(), data, writer_2, err_fn, i64)
            }
            cpal::SampleFormat::U64 => {
                generate_input_stream!(self, cfg.into(), data, writer_2, err_fn, u64)
            }
            cpal::SampleFormat::F64 => {
                generate_input_stream!(self, cfg.into(), data, writer_2, err_fn, f64)
            }
            _ => todo!(),
        };

        stream.play().unwrap();

        self.parsing = true;
        self.stream = Some(stream);
    }

    /// Stop record, if it's processing. If it's not, then just return. If it has been stoped,
    /// then begin parsing via PocketSphinx. When finished, [speech_parsed] signal will be emitted.
    #[func]
    pub fn stop_record_and_start_parse(&mut self) {
        // If there is nothing to stop, then just return
        if let None = self.stream {
            return;
        }

        // Stop autio capturing (other thread)
        self.stream = None;
        if let Some(writer) = &mut self.writer {
            writer.lock().unwrap().take().unwrap().finalize().unwrap();
        }

        godot_print!("Recording complete! Begin parsing...");

        // FIXME Filename must be the exact as recorded one.

        // Begin audio parsing (other thread)
        let data = PocketSphinxCmdData {
            pocketsphinx_path: self.pocketsphinx_path.to_string(),
            acoustic_model_path: self.acoustic_model_path.to_string(),
            language_model_path: self.language_model_path.to_string(),
            dictionary_path: self.dictionary_path.to_string(),
            filename_path: self.filename_to_record.to_string(),
        };

        self.command_sender.send(data).unwrap();
    }

    /// Signal emited when speech recognition process is finished.
    #[signal]
    fn speech_parsed(raw_json: GString);
}

#[godot_api]
impl INode for SpeechRecognitionComponent {
    fn init(base: Base<Node>) -> Self {
        // Create cpal host driver
        let host = cpal::default_host();

        // Find default input device
        let device = host.default_input_device().expect("No input device found.");

        // FIXME Workaround with unwrap
        godot_print!("Input device: {}", device.name().unwrap());

        let (tx_command, rx_command) = channel::<PocketSphinxCmdData>();
        let (tx_tokens, rx_tokens) = channel::<Tokens>();

        std::thread::spawn(move || loop {
            match &rx_command.recv() {
                Ok(data) => {
                    let output = Command::new(&data.pocketsphinx_path)
                        .arg("single")
                        .arg(&data.filename_path)
                        .arg("-hmm")
                        .arg(&data.acoustic_model_path)
                        .arg("-lm")
                        .arg(&data.language_model_path)
                        .arg("-dict")
                        .arg(&data.dictionary_path)
                        .output()
                        .expect("failed to execute process");

                    tx_tokens
                        .send(vec![String::from_utf8(output.stdout).unwrap()])
                        .unwrap();
                }
                Err(_) => (),
            }
        });

        Self {
            windows_pocketsphinx_path: GString::new(),
            linux_pocketsphinx_path: GString::new(),
            acoustic_model_path: GString::new(),
            language_model_path: GString::new(),
            dictionary_path: GString::new(),
            filename_to_record: GString::new(),
            _host: host,
            device,
            stream: None,
            writer: None,
            parsing: false,
            command_sender: tx_command,
            tokens_receiver: rx_tokens,
            base,
            pocketsphinx_path: "".to_string(),
        }
    }

    fn ready(&mut self) {
        let os = Os::singleton();
        let os_name = os.get_name().to_string();

        if &os_name == "Windows" {
            self.pocketsphinx_path = self.windows_pocketsphinx_path.to_string();
        } else if &os_name == "Linux" {
            self.pocketsphinx_path = self.linux_pocketsphinx_path.to_string();
        } else {
            godot_error!("Unsupported operating system!");
            panic!("Unsupported operating system!");
        }
    }

    fn physics_process(&mut self, _: f64) {
        if let Ok(tokens) = self.tokens_receiver.try_recv() {
            godot_print!("{tokens:?}");
            self.parsing = false;
            self.base.emit_signal("speech_parsed".into(), &[Variant::from(tokens[0].clone())]);
        }
    }
}

type WavWriterHandle = Arc<Mutex<Option<hound::WavWriter<BufWriter<File>>>>>;

/// Write wav data via writer.
fn write_input_data<T, U>(input: &[T], writer: &WavWriterHandle)
where
    T: Sample,
    U: Sample + hound::Sample + FromSample<T>,
{
    if let Ok(mut guard) = writer.try_lock() {
        if let Some(writer) = guard.as_mut() {
            for &sample in input.iter() {
                let sample: U = U::from_sample(sample);
                writer.write_sample(sample).ok();
            }
        }
    }
}
