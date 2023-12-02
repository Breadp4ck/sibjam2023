using Godot;
using System;
using System.Threading.Tasks;

public partial class AoESpell : Spell
{
	[Export] private float _speed;

	[Export] private bool _destroyOnCollie;
	
	private Vector3 _direction;

	private bool _hasToMove;

	public override void _Process(double delta)
	{
		if (_hasToMove == false)
		{
			return;
		}
		
		Move(_direction * (float)delta * _speed);
	}
	
	public override async void Cast()
	{
		_hasToMove = true;

		await Task.Delay((int)(Duration * 1000));
		
		QueueFree();
	}

	public void SetDirection(Vector3 direction)
	{
		_direction = direction;
	}
	
	// Direction must be multiplied by delta and speed. 
	private void Move(Vector3 direction)
	{
		Position += direction;
	}
}
