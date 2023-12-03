using Godot;
using System;
using System.Threading.Tasks;

public partial class Enemy : CharacterBody3D
{
	protected Vector3 Impulse = Vector3.Zero;
	protected EnemyState State = EnemyState.Idle;

	[Export] private NavigationAgent3D _navigationAgent;
	
	// Set me in editor or using SetTarget method (e.g. EnemySpawner spawns and invokes this method).
	[Export] protected Player Target;
	
	public float Speed => _speed;
	[Export] private float _speed;
	public float AttackRange => _attackRange;
	[Export] private float _attackRange;
	[Export] private float _viewDistance;
	[Export] private float _forgetDistanceIfChasing;
	[Export] private float _runAwayDistance;
	[Export] private float _startAttackTimeSeconds;
	[Export] private float _endAttackTimeSeconds;
	[Export] private float _attackCooldownSeconds;

	[Export] private HitArea _hitArea;
	
	[Export] private Node3D[] _patrolPoints;
	
	private Node3D _selectedPatrolPoint;
	
	private float _gravityForce = 20f;

	protected bool BlockStateMachine;
	private bool _isAttackOnCooldown;
	
	public override void _PhysicsProcess(double delta)
	{
		if (IsOnFloor() && BlockStateMachine == false)
		{
			switch (State)
			{
				case EnemyState.Idle:
					Idle();
					break;
				case EnemyState.Patrol:
					Patrol();
					break;
				case EnemyState.Chase:
					ChasePlayer();
					break;
				case EnemyState.Runaway:
					Runaway();
					break;
				case EnemyState.StartAttack:
					StartAttack();
					break;
				case EnemyState.Attack:
					Attack();
					break;
				case EnemyState.EndAttack:
					EndAttack();
					break;
				case EnemyState.Dead: // Do nothing.
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		else
		{
			Velocity -= new Vector3(0.0f, _gravityForce, 0.0f) * (float)delta;
		}

		Velocity += Impulse;
		Impulse = Vector3.Zero;
		MoveAndSlide();
	}

	public void SetTarget(Player target)
	{
		Target = target;
	}

	public void SetSpeed(float speed)
	{
		_speed = speed;
	}

	private void Die()
	{
		GD.Print(this + " dead!");
		State = EnemyState.Dead;
		
		// Animator play.
		Destroy();
	}

	// Vlad, call me after DEATH animation is finished.
	private void Destroy()
	{
		QueueFree();
	}

	#region StateMachine

	protected void Idle()
	{
		State = EnemyState.Patrol;
		
		int randomIndex = Random.Shared.Next(0, _patrolPoints.Length);
		_selectedPatrolPoint = _patrolPoints[randomIndex];
	}
	
	protected void Patrol()
	{	
		if (Target.GlobalPosition.DistanceTo(GlobalPosition) <= _viewDistance)
		{
			State = EnemyState.Chase;
			return;
		}

		_navigationAgent.TargetPosition = _selectedPatrolPoint.GlobalPosition;
		
		if (_navigationAgent.IsNavigationFinished() == true)
		{
			State = EnemyState.Idle;
			return;
		}
		
		Vector3 nextNavigationPoint = _navigationAgent.GetNextPathPosition();
		MoveTo(nextNavigationPoint);
	}
	
	protected void ChasePlayer()
	{
		if (Target.GlobalPosition.DistanceTo(GlobalPosition) > _forgetDistanceIfChasing)
		{
			State = EnemyState.Patrol;
			return;
		}
		
		if (Target.GlobalPosition.DistanceTo(GlobalPosition) <= _attackRange && _isAttackOnCooldown == false)
		{
			State = EnemyState.StartAttack;
			return;
		}
		
		if (Target.GlobalPosition.DistanceTo(GlobalPosition) <= _runAwayDistance && _isAttackOnCooldown == true)
		{
			State = EnemyState.Runaway;
			return;
		}
		
		_navigationAgent.TargetPosition = Target.GlobalPosition;
		Vector3 nextNavigationPoint = _navigationAgent.GetNextPathPosition();
		MoveTo(nextNavigationPoint);
		
		LookAt(new Vector3(Target.GlobalPosition.X, GlobalPosition.Y, Target.GlobalPosition.Z), Vector3.Up);
	}
	
	protected virtual void Runaway()
	{
		float distanceToPlayer = Target.GlobalPosition.DistanceTo(GlobalPosition);
		if (distanceToPlayer > _runAwayDistance && _isAttackOnCooldown == false)
		{
			State = EnemyState.Chase;
			return;
		}

		if (distanceToPlayer < _runAwayDistance)
		{
			MoveTo(GlobalPosition - Target.GlobalPosition);
		}
		
		LookAt(new Vector3(Target.GlobalPosition.X, GlobalPosition.Y, Target.GlobalPosition.Z), Vector3.Up);
	}
	
	protected virtual async void StartAttack()
	{
		BlockStateMachine = true;

		var timePassedSeconds = 0f;
		var stepMs = 50;
		while (timePassedSeconds < _startAttackTimeSeconds)
		{
			await Task.Delay(stepMs);
			timePassedSeconds += stepMs / 1000f;

			if (Target.GlobalPosition.DistanceTo(GlobalPosition) > _attackRange)
			{
				BlockStateMachine = false;
				State = EnemyState.Chase;
				return;
			}
			
			LookAt(new Vector3(Target.GlobalPosition.X, GlobalPosition.Y, Target.GlobalPosition.Z), Vector3.Up);
		}
		
		BlockStateMachine = false;
		State = EnemyState.Attack;
	}

	protected virtual async void AttackInternal()
	{
		_hitArea.Monitoring = true;
		await Task.Delay(100);
		_hitArea.Monitoring = false;
	}
	
	protected async void Attack()
	{
		AttackInternal();
		State = EnemyState.EndAttack;
		
		_isAttackOnCooldown = true;
		await Task.Delay((int)(_attackCooldownSeconds * 1000));
		_isAttackOnCooldown = false;
	}
	
	protected virtual async void EndAttack()
	{
		BlockStateMachine = true;
		await Task.Delay((int)(_endAttackTimeSeconds * 1000));
		BlockStateMachine = false;

		State = EnemyState.Chase;
	}
	
	#endregion

	private void MoveTo(Vector3 nextNavigationPoint)
	{
		Velocity = (nextNavigationPoint - GlobalPosition).Normalized() * _speed * Timescale.Enemy;
		MoveAndSlide();
	}

	private void CanAttack()
	{
		
	}
	
	public void ApplyImpulse(Vector3 impulse)
	{
		Impulse += impulse;
	}
}
