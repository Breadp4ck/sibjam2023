extends FiniteState

@export var player: CharacterBody3D

@export var FALL_GRAVITY: float = 9.81
@export var FALL_SOAR: float = 1.0

# --------------------------------------------------------------------------------------------------

func _physics_update(ctx: FiniteStateContext, delta: float) -> void:
	if not Input.is_action_pressed("slide"):
		ctx.jump_to("Fall")
		
	if player.is_on_floor():
		ctx.jump_to("Slide")
	
	player.velocity.y -= FALL_GRAVITY * FALL_SOAR * delta
