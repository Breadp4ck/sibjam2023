using Godot;
using System;
using System.Threading.Tasks;

public partial class Zombie : Enemy
{
	[Export] private float _chargeSpeed;

	// Almost base StartAttack but with velocity reset.
	protected override async void StartAttack()
	{
		BlockStateMachine = true;

		var timePassedSeconds = 0f;
		var stepMs = 50;
		while (timePassedSeconds < StartAttackTimeSeconds)
		{
			await Task.Delay(stepMs);
			timePassedSeconds += stepMs / 1000f;

			if (Target.GlobalPosition.DistanceTo(GlobalPosition) > AttackRange)
			{
				BlockStateMachine = false;
				State = EnemyState.Chase;
				return;
			}
			
			Velocity = Vector3.Zero;
			MoveAndSlide();
		}
		
		BlockStateMachine = false;
		State = EnemyState.Attack;
	}

	protected override async void AttackInternal()
	{
		HitArea.Monitoring = true;
		

		Vector3 direction = Target.GlobalPosition - GlobalPosition;

		direction.Y = 0.05f;
		direction = direction.Normalized();
		
		ApplyImpulse(direction * _chargeSpeed);

		State = EnemyState.EndAttack;
		
		await Task.Delay((int)EndAttackTimeSeconds * 1000);
		HitArea.Monitoring = false;
	}
}
