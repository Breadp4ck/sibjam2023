using Godot;
using System;

public partial class Horcrux : Area3D
{
    public static event Action OnHorcruxDie;

    private void Die()
    {
        OnHorcruxDie?.Invoke();
        // Call Destroy() after animation ends.
        Destroy(); // TODO: Delete
    }

    // Animator.
    private void Destroy()
    {
        QueueFree();
    }
}
