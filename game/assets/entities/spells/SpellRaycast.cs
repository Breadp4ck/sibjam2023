using System;
using Godot;

public partial class SpellRaycast : RayCast3D
{
	[Export] private SpellCaster _spellCaster;
	
	public override void _PhysicsProcess(double delta)
	{
		if (IsColliding() == false)
		{
			return;
		}

		Area3D area = GetCollider() as Area3D;
		Enemy enemy;
		try
		{
			enemy = area?.GetParent<Enemy>();
		}
		catch (Exception e)
		{
			return;
			// ignored
		}

		if (enemy == null)
		{
			return;
		}
		
		_spellCaster.SetTarget(enemy);
	}
}
