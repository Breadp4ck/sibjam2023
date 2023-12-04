using Godot;
using System;

public partial class PickableRaycast : RayCast3D
{
    public static PickableSpell CurrentPickableSpell { get; private set; } 
    
    public override void _PhysicsProcess(double delta)
    {
        if (IsColliding() == false)
        {
            CurrentPickableSpell = null;
            return;
        }

        Area3D area = GetCollider() as Area3D;
        PickableSpell pickableSpell = (PickableSpell)area;
		
        if (pickableSpell == null || CurrentPickableSpell == pickableSpell)
        {
            return;
        }

        CurrentPickableSpell = pickableSpell;
        GD.Print($"PickableSpell ({pickableSpell.Name}) is in range. Press F or ?smth? to pick it up.");
    }
}
