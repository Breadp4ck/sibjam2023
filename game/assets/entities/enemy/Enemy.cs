using Godot;
using System;
using System.Threading.Tasks;

public partial class Enemy : CharacterBody3D
{
	protected Vector3 Impulse = Vector3.Zero;
	protected EnemyState State = EnemyState.Idle;
	
	[Export] public Vector3 RealPosition; // Offset from the 0 coords to 'real' enemy position.
	
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
	[Export] protected float StartAttackTimeSeconds;
	[Export] protected float EndAttackTimeSeconds;
	[Export] private float _attackCooldownSeconds;

	[Export] protected HitArea HitArea;
	
	[Export] private Node3D[] _patrolPoints;
	
	private Node3D _selectedPatrolPoint;
	
	private float _gravityForce = 20f;

	protected bool BlockStateMachine;
	protected bool IsAttackOnCooldown;

	public override void _Process(double delta)
	{
		LookAt(new Vector3(Target.GlobalPosition.X, GlobalPosition.Y, Target.GlobalPosition.Z), Vector3.Up);
	}

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

	protected virtual void Idle()
	{
		State = EnemyState.Patrol;
		
		if (_patrolPoints.Length == 0)
		{
			SetViewDistance(50);
			SetForgetDistanceIfChasing(50);
			State = EnemyState.Chase;
			return;
		}
		
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
		
		if (Target.GlobalPosition.DistanceTo(GlobalPosition) <= _attackRange && IsAttackOnCooldown == false)
		{
			State = EnemyState.StartAttack;
			return;
		}
		
		if (Target.GlobalPosition.DistanceTo(GlobalPosition) <= _runAwayDistance && IsAttackOnCooldown == true)
		{
			State = EnemyState.Runaway;
			return;
		}
		
		_navigationAgent.TargetPosition = Target.GlobalPosition;
		Vector3 nextNavigationPoint = _navigationAgent.GetNextPathPosition();
		MoveTo(nextNavigationPoint);
	}
	
	protected virtual void Runaway()
	{
		float distanceToPlayer = Target.GlobalPosition.DistanceTo(GlobalPosition);
		if (distanceToPlayer > _runAwayDistance && IsAttackOnCooldown == false)
		{
			State = EnemyState.Chase;
			return;
		}

		if (distanceToPlayer < _runAwayDistance)
		{
			MoveTo(GlobalPosition - Target.GlobalPosition);
		}
	}
	
	protected virtual async void StartAttack()
	{
		BlockStateMachine = true;

		var timePassedSeconds = 0f;
		var stepMs = 50;
		while (timePassedSeconds < StartAttackTimeSeconds)
		{
			await Task.Delay(stepMs);
			timePassedSeconds += stepMs / 1000f;

			if (Target.GlobalPosition.DistanceTo(GlobalPosition) > _attackRange)
			{
				BlockStateMachine = false;
				State = EnemyState.Chase;
				return;
			}
		}
		
		BlockStateMachine = false;
		State = EnemyState.Attack;
	}

	protected virtual async void AttackInternal()
	{
		HitArea.Monitoring = true;
		await Task.Delay((int)EndAttackTimeSeconds * 1000);
		HitArea.Monitoring = false;
	}
	
	protected async void Attack()
	{
		AttackInternal();
		State = EnemyState.EndAttack;
		
		IsAttackOnCooldown = true;
		await Task.Delay((int)(_attackCooldownSeconds * 1000));
		IsAttackOnCooldown = false;
	}
	
	protected virtual async void EndAttack()
	{
		BlockStateMachine = true;
		await Task.Delay((int)(EndAttackTimeSeconds * 1000));
		BlockStateMachine = false;

		State = EnemyState.Chase; // TODO: Can be switched to Idle?
	}
	
	#endregion

	private void MoveTo(Vector3 nextNavigationPoint)
	{
		Velocity = (nextNavigationPoint - GlobalPosition).Normalized() * _speed * Timescale.Enemy;
		MoveAndSlide();
	}

	private void SetViewDistance(float viewDistance)
	{
		_viewDistance = viewDistance;
	}

	private void SetForgetDistanceIfChasing(float forgetDistanceIfChasing)
	{
		_forgetDistanceIfChasing = forgetDistanceIfChasing;
	}
	
	public void ApplyImpulse(Vector3 impulse)
	{
		Impulse += impulse;
	}
}
