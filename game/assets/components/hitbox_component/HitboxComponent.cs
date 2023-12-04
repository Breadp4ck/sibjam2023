using System;
using Godot;

public partial class HitboxComponent : Area3D
{
	public event Action<double> HpChangedEvent;
    
    [Export] private HealthComponent _healthComponent;
    
    public void Hit(double damage)
    {
        _healthComponent.Hit(damage);
        HpChangedEvent?.Invoke(_healthComponent.Health);
    }
}
