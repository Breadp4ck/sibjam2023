extends FiniteState

@export var player: CharacterBody3D
@export var collision: CollisionShape3D
@export var camera: Node3D
@export var shape_cast: ShapeCast3D

@export var CROUCH_SPEED: float = 5.0

func _start(_ctx: FiniteStateContext) -> void:
	camera.position.y = 0.8
	collision.shape.height = 0.9
	collision.position.y = 0.9 / 2.0


# --------------------------------------------------------------------------------------------------

func _handle_input(ctx: FiniteStateContext, event: InputEvent) -> void:
	if event.is_action_pressed("jump"):
		ctx.jump_to("Leap")
		
	elif event.is_action_pressed("slide"):
		ctx.jump_to("Slide")
		

# --------------------------------------------------------------------------------------------------

func _physics_update(ctx: FiniteStateContext, _delta: float) -> void:

	if not player.is_on_floor():
		ctx.jump_to("Fall")
		return
		
	var input := Input.get_vector("move_left", "move_right", "move_forward", "move_back")
	
	var direction := (camera.global_transform.basis * Vector3(input.x, 0, input.y))
	direction.y = 0
	direction = direction.normalized()
	
	player.velocity.x = direction.x * CROUCH_SPEED
	player.velocity.z = direction.z * CROUCH_SPEED

	if not player.is_on_floor():
		ctx.jump_to("Fall")
		return
	
	elif not Input.is_action_pressed("crouch"):
		shape_cast.force_shapecast_update()
		shape_cast.get_collision_count()
		if shape_cast.is_colliding():
			return
		else:
			ctx.jump_to("Idle")

