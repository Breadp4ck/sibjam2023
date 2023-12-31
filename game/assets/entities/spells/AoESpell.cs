using Godot;
using System;
using System.Threading.Tasks;

public abstract partial class AoESpell : Spell
{
	[Export] protected float Speed;

	[Export] private bool _destroyOnCollide;
	
	private Vector3 _direction;

	private bool _hasToMove;

	public override void _Process(double delta)
	{
		if (_hasToMove == false)
		{
			return;
		}
		
		Move(_direction * (float)delta * Speed);
	}
	
	public override async void Cast()
	{
		_hasToMove = true;

		await Task.Delay((int)(Duration * 1000));

		try
		{
			QueueFree();
		}
		catch (Exception)
		{
			// ignored
		}
	}
	
	public void SetDirection(Vector3 direction)
	{
		_direction = direction;
	}

	private void OnAreaEntered(Area3D area3D)
	{
		OnAreaEnteredInternal(area3D);

		if (_destroyOnCollide == true)
		{
			QueueFree();
		}
	}

	protected abstract void OnAreaEnteredInternal(Area3D area3D);

	// Direction must be multiplied by delta and speed. 
	private void Move(Vector3 direction)
	{
		Position += direction;
	}
}
