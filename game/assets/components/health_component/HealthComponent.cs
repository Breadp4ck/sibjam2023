using System;
using Godot;

public partial class HealthComponent : Node
{
	public static event Action<Enemy> Died;
	
	[Export] private double _maxHealth = 2;

	[Signal] public delegate void DeadEventHandler();

	public double Health => _health;
	private double _health;
	
	public override void _Ready()
	{
		_health = _maxHealth;
	}

	public void Hit(double damage)
	{
		GD.Print("Deal " + damage + " damage.");
		_health -= damage;

		if (_health > 0)
		{
			return;
		}
		
		EmitSignal(SignalName.Dead);

		Enemy enemy;
		try
		{
			enemy = GetParent<Enemy>();
		}
		catch (Exception)
		{
			return;
			// ignored
		}

		Died?.Invoke(enemy);
	}
}
