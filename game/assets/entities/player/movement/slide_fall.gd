extends FiniteState

@export var player: CharacterBody3D
@export var camera: Node3D

@export var FALL_GRAVITY: float = 9.81
@export var FALL_SOAR: float = 1.0
@export var FALL_SLIDE_SPEED: float = 20.0
@export var MAX_VELOCITY: float = 15.0

var CameraDirection: Basis

# --------------------------------------------------------------------------------------------------

func _start(ctx: FiniteStateContext) -> void:
	CameraDirection = camera.global_transform.basis

# --------------------------------------------------------------------------------------------------

func _physics_update(ctx: FiniteStateContext, delta: float) -> void:
	if not Input.is_action_pressed("slide"):
		ctx.jump_to("Fall")
		
	if player.is_on_floor():
		ctx.jump_to("Slide")
	
	var input = Vector2.ZERO
	
	if Input.is_action_pressed("move_forward") or input == Vector2.ZERO:
		input = Vector2(0, -1)
		
	if Input.is_action_pressed("move_back"):
		input = Vector2(0, 1)
		
	if Input.is_action_pressed("move_left"):
		input.x = cos(PI * 135 / 180)
		input.y = -sin(PI * 135 / 180)
		
	if Input.is_action_pressed("move_right"):
		input.x = cos(PI * 45 / 180)
		input.y = -sin(PI * 45 / 180)
		
	if Input.is_action_pressed("move_left") and Input.is_action_pressed("move_back"):
		input.x = cos(PI * 135 / 180)
		input.y = sin(PI * 135 / 180)
		
	if Input.is_action_pressed("move_right") and Input.is_action_pressed("move_back"):
		input.x = cos(PI * 45 / 180)
		input.y = sin(PI * 45 / 180)
	
	var direction := (CameraDirection * Vector3(input.x, 0, input.y))
	direction.y = 0
	direction = direction.normalized()
	
	var ground_velocity = player.velocity
	ground_velocity.y = 0
	
	var speed = ground_velocity.length()
	
	player.velocity.x = direction.x * speed
	player.velocity.z = direction.z * speed
	
	player.velocity.y -= FALL_GRAVITY * FALL_SOAR * delta
	
	player.velocity.x = clampf(player.velocity.x, -MAX_VELOCITY, MAX_VELOCITY)
	player.velocity.z = clampf(player.velocity.z, -MAX_VELOCITY, MAX_VELOCITY)
