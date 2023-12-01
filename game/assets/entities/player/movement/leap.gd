extends FiniteState

@export var player: CharacterBody3D
@export var collision: CollisionShape3D
@export var camera: Camera3D

@export var JUMP_POWER: float = 5.0

func _start(ctx: FiniteStateContext) -> void:
	camera.position.y = 1.7
	collision.shape.height = 1.8
	collision.position.y = 1.8 / 2.0
	
	player.velocity += Vector3(0.0, JUMP_POWER, 0.0)
	ctx.jump_to("Fall")
