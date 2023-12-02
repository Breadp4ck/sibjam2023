extends FiniteState

@export var player: CharacterBody3D
@export var collision: CollisionShape3D
@export var camera: Camera3D

@export var SLIDE_SPEED: float = 20.0
@export var SLIDE_FADING: float = 2.0

# --------------------------------------------------------------------------------------------------

func _start(ctx: FiniteStateContext) -> void:
	camera.position.y = 0.8
	collision.shape.height = 0.9
	collision.position.y = 0.9 / 2.0
	
	var input := Input.get_vector("move_left", "move_right", "move_forward", "move_back")
	
	if input == Vector2.ZERO:
		input = Vector2(0.0, -1.0)
	
	var direction := (camera.global_transform.basis * Vector3(input.x, 0, input.y))
	direction.y = 0
	direction = direction.normalized()
	
	player.velocity.x = direction.x * SLIDE_SPEED
	player.velocity.z = direction.z * SLIDE_SPEED

# --------------------------------------------------------------------------------------------------

func _handle_input(ctx: FiniteStateContext, event: InputEvent) -> void:
	if event.is_action_pressed("jump"):
		ctx.jump_to("SlideJump")
		
	elif not Input.is_action_pressed("slide"):
		if Input.get_vector("move_left", "move_right", "move_forward", "move_back") == Vector2.ZERO:
			ctx.jump_to("Walk")
		else:
			ctx.jump_to("Idle")
			
	elif not player.is_on_floor():
		ctx.jump_to("SlideFall")
	
