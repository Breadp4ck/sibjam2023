using Godot;
using System;

public partial class Horcrux : Area3D
{
    public static event Action OnHorcruxDie;
    
    private void OnAreaEntered(Area3D area3D)
    {
        OnHorcruxDie?.Invoke();
        // Animator state change.
        // TODO: Delete me.
        Die();
    }
    
    // Animator.
    private void Die()
    {
        QueueFree();
    }
}
