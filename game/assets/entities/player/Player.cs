using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public SpellSelectType SpellSelectType => _spellPresenter.SpellSelectType;
	
	[Export] private SpellPresenter _spellPresenter;

	public float WalkSpeed => _walkSpeed * Timescale.Player;
	[Export] private float _walkSpeed = 10.0f;

	[Export] private HitboxComponent _hitboxComponent;
	
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}

	public void Hit(uint damage)
	{
		_hitboxComponent.Hit(damage);
	}
	
	private void Die()
	{
		GD.Print("Player is dead!");

		// Animator play.
		Destroy();
	}

	private void Destroy()
	{
		QueueFree();
	}
}
