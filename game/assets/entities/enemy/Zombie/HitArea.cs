using Godot;
using System;

public partial class HitArea : Area3D
{
    [Export] private double _damage;
    
    private void OnAreaEntered(Area3D area3D)
    {
        HitboxComponent hitboxComponent = (HitboxComponent)area3D;
        hitboxComponent.Hit(_damage);
        GD.Print(hitboxComponent.Name);
    }
}
