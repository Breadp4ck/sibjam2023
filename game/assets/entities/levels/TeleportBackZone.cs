using Godot;
using System;

public partial class TeleportBackZone : Area3D
{
    [Export] private Node3D _teleportBackPosition;
    
    private void OnAreaEntered(Area3D area3D)
    {
        area3D.GetParent<Player>().GlobalPosition = _teleportBackPosition.GlobalPosition;
    }
}
