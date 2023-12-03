extends FiniteState

@export var player: CharacterBody3D
@export var collision: CollisionShape3D
@export var camera: Node3D
@export var shape_cast : ShapeCast3D
@export var shape_cast_walls : ShapeCast3D

@export var SLIDE_SPEED: float = 20.0
@export var SLIDE_DURATION: float = 3.0
@export var TURN_RATE: float = 4.0

var SlideTime: float
var BaseVelocity: Vector3
var CameraDirection: Basis

# --------------------------------------------------------------------------------------------------

func _start(ctx: FiniteStateContext) -> void:
	camera.position.y = 0.8
	collision.shape.height = 0.9
	collision.position.y = 0.9 / 2.0
	
	SlideTime = 0.0
	
	var input := Input.get_vector("move_left", "move_right", "move_forward", "move_back")
	
	if input == Vector2.ZERO:
		input = Vector2(0.0, -1.0)
	
	var direction := (camera.global_transform.basis * Vector3(input.x, 0, input.y))
	direction.y = 0
	direction = direction.normalized()
	CameraDirection = camera.global_transform.basis
	
	var ground_velocity = player.velocity
	ground_velocity.y = 0
	
	var speed = ground_velocity.length()
	
	player.velocity.x = direction.x * speed
	player.velocity.z = direction.z * speed
	BaseVelocity = player.velocity

# --------------------------------------------------------------------------------------------------

func _physics_update(ctx: FiniteStateContext, _delta: float) -> void:
	SlideTime += float(_delta)
	var t = (SLIDE_DURATION - SlideTime) / SLIDE_DURATION
	player.velocity.x = BaseVelocity.x * t * t * t
	player.velocity.z = BaseVelocity.z * t * t * t
	
	var input = Vector2.ZERO
	
	if Input.is_action_pressed("move_left"):
		input.x = cos(PI * 135 / 180)
		input.y = -sin(PI * 135 / 180)
		
	if Input.is_action_pressed("move_right"):
		input.x = cos(PI * 45 / 180)
		input.y = -sin(PI * 45 / 180)
	
	var direction := (CameraDirection * Vector3(input.x, 0, input.y))
	direction.y = 0
	direction = direction.normalized()
	
	var ground_velocity = player.velocity
	ground_velocity.y = 0
	
	var speed = ground_velocity.length()
	
	if t > 0.3:
		player.velocity.x += direction.x * TURN_RATE
		player.velocity.z += direction.z * TURN_RATE
	
	shape_cast_walls.force_shapecast_update()
	shape_cast_walls.get_collision_count()
	if shape_cast_walls.is_colliding():
		ctx.jump_to("Crouch")
	
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
		
	if SlideTime >= SLIDE_DURATION:
		ctx.jump_to("Idle")
