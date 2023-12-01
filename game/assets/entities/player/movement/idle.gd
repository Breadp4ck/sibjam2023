extends FiniteState

@export var player: CharacterBody3D
@export var collision: CollisionShape3D
@export var camera: Camera3D

func _start(_ctx: FiniteStateContext) -> void:
	player.velocity = Vector3.ZERO
	
	camera.position.y = 1.7
	collision.shape.height = 1.8
	collision.position.y = 1.8 / 2.0

# --------------------------------------------------------------------------------------------------

func _handle_input(ctx: FiniteStateContext, event: InputEvent) -> void:
	if event.is_action_pressed("slide"):
		ctx.jump_to("Slide")
	
	elif event.is_action_pressed("jump"):
		ctx.jump_to("Leap")

# --------------------------------------------------------------------------------------------------

func _physics_update(ctx: FiniteStateContext, _delta: float) -> void:
	if Input.get_vector("move_left", "move_right", "move_forward", "move_back") != Vector2.ZERO:
		ctx.jump_to("Walk")
		return
		
	if not player.is_on_floor():
		ctx.jump_to("Fall")
