using Godot;
using System;
using System.Threading.Tasks;

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

	private async void Die()
	{
		GD.Print("Player is dead!");

		Inventory.Clear();
		Control screen = (Control)GetTree().GetFirstNodeInGroup("DeathScreen");
		screen.Visible = true;
		await Task.Delay(4000);
		GetTree().ReloadCurrentScene();
	}
}
