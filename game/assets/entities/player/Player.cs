using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public float WalkSpeed => _walkSpeed * Timescale.Player;
	[Export] private float _walkSpeed = 10.0f;
	
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}

	private void Die()
	{
	}
}
