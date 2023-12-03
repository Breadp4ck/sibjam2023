using Godot;

public partial class HitboxComponent : Area3D
{
    [Export] private HealthComponent _healthComponent;
    
    public void Hit(double damage)
    {
        _healthComponent.Hit(damage);
    }
}
