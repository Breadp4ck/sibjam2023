extends GPUParticles3D

@onready var player = $AudioStreamPlayer
@onready var timer  = $Timer
@onready var animation = $OmniLight3D/AnimationPlayer

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_timer_timeout():
	player.play()
	self.emitting = true
	animation.play("light")
	timer.start(randf() * 4 + 1)
