using Godot;
using System;

public partial class FlyingEnemy : Enemy
{
    [Export] private PackedScene _projectile;

    public override void _PhysicsProcess(double delta)
    {

        if (BlockStateMachine == true)
        {
            return;
        }
        
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

    protected override void AttackInternal()
    {
        Projectile projectile = _projectile.Instantiate() as Projectile;
        
        if (projectile == null)
        {
            return;
        }

        GetTree().Root.AddChild(projectile);
        projectile.GlobalPosition = GlobalPosition + RealPosition;
        projectile.SetDirection(Target.GlobalPosition + Vector3.Up - projectile.GlobalPosition);
        projectile.Cast();
    }
}
