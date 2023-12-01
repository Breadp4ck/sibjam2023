extends FiniteState

@export var player: CharacterBody3D

@export var JUMP_POWER: float = 10.0

func _start(ctx: FiniteStateContext) -> void:
	player.velocity += Vector3(0.0, JUMP_POWER, 0.0)
	ctx.jump_to("SlideFall")
