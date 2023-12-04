using Godot;
using System;

public partial class Projectile : AoESpell
{
    [Export] private float _damage;
    
    protected override void OnAreaEnteredInternal(Area3D area3D)
    {
        HitboxComponent hitboxComponent = (HitboxComponent)area3D;
        hitboxComponent.Hit(_damage);
    }
}
