using Godot;
using System;

public partial class FlyingEnemy : Enemy
{
    [Export] private PackedScene _projectile;
    [Export] private Vector3 _realPosition; // Offset from the 0 coords to 'real' enemy position.

    public override void _PhysicsProcess(double delta)
    {

        if (BlockStateMachine == true)
        {
            return;
        }
        
        GD.Print(State);
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
        Projectile miniSpell = _projectile.Instantiate() as Projectile;
        
        if (miniSpell == null)
        {
            return;
        }

        GetTree().Root.AddChild(miniSpell);
        miniSpell.GlobalPosition = GlobalPosition + _realPosition;
        miniSpell.SetDirection(Target.GlobalPosition - GlobalPosition);
        miniSpell.Cast();
    }
}
