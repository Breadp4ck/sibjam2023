using Godot;
using System.Threading.Tasks;

public partial class SlowMob : TargetSpell
{
	[Export] private float _durationAfterApplied;
	[Export] private float _newSpeed;

	public override async void Cast()
	{
		HasToMove = true;
		
		var timePassedSeconds = 0f;
		const int stepMs = 50;
        
		// Wait for whole Duration or until IsApplied is true - then wait for applied duration.
		while (timePassedSeconds < Duration)
		{
			timePassedSeconds += stepMs / 1000f;
            
			if (IsApplied == true)
			{
				await Task.Delay((int)(_durationAfterApplied * 1000));
				break;
			}

			await Task.Delay(stepMs);
		}
		
		QueueFree();
	}

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

		IsApplied = true;

		await Task.Delay((int)(_durationAfterApplied * 1000));

		enemy.SetSpeed(originalSpeed);
	}
}
