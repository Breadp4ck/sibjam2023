extends Node

@onready var player = $".."
@onready var speech_recognition: SpeechRecognitionComponent = $SpeechRecognitionComponent

var is_recording: bool

signal speech_parsed(text: String)

func _input(event):
	if player.get("SpellSelectType") != 1:
		return
	
	if event.is_action_pressed("microphone") and !is_recording:
		start()
	elif event.is_action_released("microphone") and is_recording:
		stop()

# Begin recording
func start() -> void:
	speech_recognition.start_record()
	is_recording = true

# Stop record and begin pocketsphinx processing
func stop() -> void:
	speech_recognition.stop_record_and_start_parse()
	is_recording = false

func _on_speech_recognition_component_speech_parsed(raw_json: String) -> void:
	var json: Dictionary = JSON.parse_string(raw_json)
	var text: String = json.t
	
	print("Speech parsed:", text)
	speech_parsed.emit(text)
