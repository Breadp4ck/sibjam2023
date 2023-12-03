extends FiniteState

@export var player: CharacterBody3D
@export var collision: CollisionShape3D
@export var camera: Node3D

@export var FALL_WALK_SPEED: float = 10.0
@export var FALL_GRAVITY: float = 9.81
@export var FALL_SOAR: float = 1.0

func _start(_ctx: FiniteStateContext) -> void:
	camera.position.y = 1.7
	collision.shape.height = 1.8
	collision.position.y = 1.8 / 2.0

# --------------------------------------------------------------------------------------------------

func _physics_update(ctx: FiniteStateContext, delta: float) -> void:
	if player.is_on_floor():
		if Input.is_action_pressed("slide"):
			ctx.jump_to("Slide")
		elif Input.is_action_pressed("crouch"):
			ctx.jump_to("Crouch")
		else:
			ctx.jump_to("Idle")
		return
	
	var input := Input.get_vector("move_left", "move_right", "move_forward", "move_back")
	
	var direction := (camera.global_transform.basis * Vector3(input.x, 0, input.y))
	direction.y = 0
	direction = direction.normalized()
	
	player.velocity.x = direction.x * FALL_WALK_SPEED
	player.velocity.z = direction.z * FALL_WALK_SPEED
	
	player.velocity.y -= FALL_GRAVITY * FALL_SOAR * delta
