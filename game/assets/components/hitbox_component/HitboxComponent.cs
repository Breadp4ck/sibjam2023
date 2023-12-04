using System;
using Godot;

public partial class HitboxComponent : Area3D
{
	public event Action<double> HPLostEvent;
    
    [Export] private HealthComponent _healthComponent;
    
    public void Hit(double damage)
    {
        _healthComponent.Hit(damage);
        HPLostEvent?.Invoke(damage);
    }
}
