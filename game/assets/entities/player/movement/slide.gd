extends FiniteState

@export var player: CharacterBody3D
@export var collision: CollisionShape3D
@export var camera: Node3D
@export var shape_cast : ShapeCast3D

@export var SLIDE_SPEED: float = 20.0
@export var SLIDE_FADING: float = 2.0
@export var SLIDE_DURATION: float = 4.0

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

func _physics_update(ctx: FiniteStateContext, _delta: float) -> void:
	SLIDE_DURATION -= float(_delta)
	print(SLIDE_DURATION)
	if Input.is_action_pressed("jump"):
		ctx.jump_to("SlideJump")
		
	elif not Input.is_action_pressed("slide"):
		shape_cast.force_shapecast_update()
		shape_cast.get_collision_count()
		if shape_cast.is_colliding():
			ctx.jump_to("Crouch")
		else:
			if Input.get_vector("move_left", "move_right", "move_forward", "move_back") == Vector2.ZERO:
				ctx.jump_to("Walk")
			elif Input.is_action_pressed("crouch"):
				ctx.jump_to("Crouch")
			else:
				ctx.jump_to("Idle")
			
	elif not player.is_on_floor():
		ctx.jump_to("SlideFall")
