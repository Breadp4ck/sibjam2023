using Godot;
using System;
using System.Threading.Tasks;

public partial class Zombie : Enemy
{
    [Export] private float _chargeTime;
    [Export] private float _chargeSpeed;

    protected override async void Attack()
    {
        var timePassedSeconds = 0f;
        var stepMs = 50;
        while (timePassedSeconds < _chargeTime)
        {
            await Task.Delay(stepMs);
            timePassedSeconds += stepMs / 1000f;

            Velocity = Vector3.Zero;
            MoveAndSlide();
        }

        Vector3 direction = Target.GlobalPosition - GlobalPosition;

        direction.Y = 0.05f;
        direction = direction.Normalized();
        
        AttackInternal();
        ApplyImpulse(direction * _chargeSpeed);

        State = EnemyState.EndAttack;
    }
}
