using Godot;
using System;
using System.Threading.Tasks;

public partial class Dummy : CharacterBody3D
{
	private double _time;

	public override void _PhysicsProcess(double delta)
	{
		_time += delta;
		Velocity = new Vector3(5*(float)Math.Sin(_time) * Timescale.Scale, 0, 0);
		MoveAndSlide();
	}
}
