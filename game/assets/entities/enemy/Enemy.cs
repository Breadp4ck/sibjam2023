using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

public partial class Enemy : CharacterBody3D
{
	[Export] private NavigationAgent3D _navigationAgent;
	
	// Set me in editor or using SetTarget method (e.g. EnemySpawner spawns and invokes this method).
	[Export] private Node3D _target;
	
	public float Speed => _speed;	
	[Export] private float _speed;
	[Export] private float _attackRange;
	[Export] private float _viewDistance;
	[Export] private float _forgetDistanceIfChasing;
	
	[Export] private Node3D[] _patrolPoints;
	private Node3D _selectedPatrolPoint;

	private EnemyState _state = EnemyState.Idle;

	private Vector3 _impulse = Vector3.Zero;

	public override void _PhysicsProcess(double delta)
	{
		if (IsOnFloor())
		{
			switch (_state)
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
				case EnemyState.Attack:
					Attack();
					break;
				case EnemyState.Dead: // Do nothing.
					break;
			}
		}

		else
		{
			Velocity -= new Vector3(0.0f, 20f, 0.0f) * (float)delta;
		}

		Velocity += _impulse;
		_impulse = Vector3.Zero;
		MoveAndSlide();
	}

	public void SetTarget(Node3D target)
	{
		_target = target;
	}

	public void SetSpeed(float speed)
	{
		_speed = speed;
	}

	// Vlad, call me in animator after ATTACK animation is finished.
	public void DealDamage()
	{
		
	}

	private void Die()
	{
		GD.Print(this + " dead!");
		_state = EnemyState.Dead;
	}

	// Vlad, call me after DEATH animation is finished.
	private void Destroy()
	{
		QueueFree();
	}

	#region StateMachine

	private void Idle()
	{
		_state = EnemyState.Patrol;
		
		int randomIndex = Random.Shared.Next(0, _patrolPoints.Length);
		_selectedPatrolPoint = _patrolPoints[randomIndex];
	}
	
	private void Patrol()
	{	
		if (_target.GlobalPosition.DistanceTo(GlobalPosition) <= _viewDistance)
		{
			_state = EnemyState.Chase;
			return;
		}

		_navigationAgent.TargetPosition = _selectedPatrolPoint.GlobalPosition;
		
		if (_navigationAgent.IsNavigationFinished() == true)
		{
			_state = EnemyState.Idle;
			return;
		}
		
		Vector3 nextNavigationPoint = _navigationAgent.GetNextPathPosition();
		MoveTo(nextNavigationPoint);
		
	}
	
	private void ChasePlayer()
	{
		if (_target.GlobalPosition.DistanceTo(GlobalPosition) > _forgetDistanceIfChasing)
		{
			_state = EnemyState.Patrol;
			return;
		}
		
		if (_target.GlobalPosition.DistanceTo(GlobalPosition) <= _attackRange)
		{
			_state = EnemyState.Attack;
			return;
		}
		
		_navigationAgent.TargetPosition = _target.GlobalPosition;
		Vector3 nextNavigationPoint = _navigationAgent.GetNextPathPosition();
		MoveTo(nextNavigationPoint);
		
		LookAt(new Vector3(_target.GlobalPosition.X, GlobalPosition.Y, _target.GlobalPosition.Z), Vector3.Up);
	}
	
	private void Attack()
	{
		if (_target.GlobalPosition.DistanceTo(GlobalPosition) > _attackRange)
		{
			_state = EnemyState.Chase;
			return;
		}

		LookAt(new Vector3(_target.GlobalPosition.X, GlobalPosition.Y, _target.GlobalPosition.Z), Vector3.Up);
	}
	#endregion

	private void MoveTo(Vector3 nextNavigationPoint)
	{
		Velocity = (nextNavigationPoint - GlobalPosition).Normalized() * _speed * Timescale.Enemy;
		//MoveAndSlide();
	}

	public void ApplyImpulse(Vector3 impulse)
	{
		_impulse += impulse;
	}
}
