using Godot;
using System;
using System.Threading.Tasks;

public partial class SlowMob : TargetSpell
{
	[Export] private float _newSpeed;
	
	protected override async void OnAreaEnteredInternal(Area3D area3D)
	{
		Enemy enemy = area3D.GetParent<Enemy>();

		if (enemy == null)
		{
			GD.Print("Failed to get Enemy.");
			return;
		}
		
		float originalSpeed = enemy.Speed;
		enemy.SetSpeed(_newSpeed);

		await Task.Delay((int)(Duration * 1000));
		
		enemy.SetSpeed(originalSpeed);
	}
}
