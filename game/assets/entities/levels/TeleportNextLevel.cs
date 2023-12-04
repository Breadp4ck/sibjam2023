using Godot;
using System;

public partial class TeleportNextLevel : Area3D
{
    [Export] private Vector3 _teleportPositionOffset;

    private void OnAreaEntered(Area3D area3D)
    {
        area3D.GetParent<Player>().GlobalPosition += _teleportPositionOffset;
    }
}
