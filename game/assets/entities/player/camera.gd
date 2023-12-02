extends Node3D

@export var sensetive_h: float = 0.2
@export var sensetive_v: float = 0.2

var camera_rotation_h: float = 0.0
var camera_rotation_v: float = 0.0

# --------------------------------------------------------------------------------------------------

func _input(event: InputEvent) -> void:
	if event is InputEventMouseMotion:
		camera_rotation_v = clampf(camera_rotation_v - event.relative.y * sensetive_v, -90.0, 90.0)
		camera_rotation_h += -event.relative.x * sensetive_h

# --------------------------------------------------------------------------------------------------

func _physics_process(_delta: float) -> void:
	self.rotation.y = deg_to_rad(camera_rotation_h)
	self.rotation.x = deg_to_rad(camera_rotation_v)
