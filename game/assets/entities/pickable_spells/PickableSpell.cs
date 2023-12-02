using Godot;
using System;

public partial class PickableSpell : Area3D
{
    [Export] private SpellType _spellType;

    public override void _Process(double delta)
    {
        if (PickableRaycast.CurrentPickableSpell != this)
        {
            return;
        }
        
        if (Input.IsActionJustPressed("interact"))
        {
            Pick();
        }
    }

    private void Pick()
    {
        Inventory.AddSpell(_spellType);
        QueueFree();
    }
}
