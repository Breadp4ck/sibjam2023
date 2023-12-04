using Godot;
using System;

public partial class InteractRaycast : RayCast3D
{
    public static SpellSelectTypeSwitcher TypeSwitcher { get; private set; } 

    public override void _PhysicsProcess(double delta)
    {
        if (IsColliding() == false)
        {
            TypeSwitcher = null;
            return;
        }

        Area3D area = GetCollider() as Area3D;
        SpellSelectTypeSwitcher typeSwitcher = (SpellSelectTypeSwitcher)area;
		
        if (typeSwitcher == null || TypeSwitcher == typeSwitcher)
        {
            return;
        }

        TypeSwitcher = typeSwitcher;
        GD.Print($"Type Switcher ({typeSwitcher.Name}) is in range. Press F or ?smth? to switch.");
    }
}
