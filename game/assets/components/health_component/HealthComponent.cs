using Godot;

public partial class HealthComponent : Node
{
	[Export] private double _maxHealth = 2;

	[Signal] public delegate void DeadEventHandler();

	private double _health;
	
	public override void _Ready()
	{
		_health = _maxHealth;
	}

	public void Hit(double damage)
	{
		GD.Print("Deal " + damage + " damage.");
		_health -= damage;

		if (_health <= 0)
		{
			EmitSignal(SignalName.Dead);
		}
	}
}
