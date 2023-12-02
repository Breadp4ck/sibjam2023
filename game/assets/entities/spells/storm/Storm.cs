using Godot;
using System;

public partial class Storm : TargetSpell
{
    [Export] private PackedScene _miniStormSpell;
    [Export] private uint _damage;
    
    protected override void OnAreaEnteredInternal(Area3D area3D)
    {
        Node3D miniSpell = _miniStormSpell.Instantiate() as Node3D;

        if (miniSpell == null)
        {
            return;
        }
        
        GetTree().Root.AddChild(miniSpell);
        miniSpell.GlobalPosition = GlobalPosition;
        
        HitboxComponent hitboxComponent = (HitboxComponent)area3D;
        hitboxComponent.Hit(_damage);
    }
}