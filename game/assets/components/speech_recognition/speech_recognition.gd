extends Node

@onready var speech_recognition: SpeechRecognitionComponent = $SpeechRecognitionComponent

signal speech_parsed(text: String)

# Begin recording
func start() -> void:
	speech_recognition.start_record()

# Stop record and begin pocketsphinx processing
func stop() -> void:
	speech_recognition.stop_record_and_start_parse()

func _on_speech_recognition_component_speech_parsed(raw_json: String) -> void:
	var json: Dictionary = JSON.parse_string(raw_json)
	var text: String = json.t
	
	print("Speech parsed:", text)
	speech_parsed.emit(text)
