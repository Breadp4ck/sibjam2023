using Godot;
using System;

public partial class TeleportNextLevel : Area3D
{
    [Export] private Node3D _teleportDestination;
    [Export] private uint _killsToOpen;

    [Export] private Node3D _particles;
    
    private bool _canTeleport;
    
    public override void _Ready()
    {
        HealthComponent.Died += OnEnemyDied;
    }
    
    private void OnEnemyDied(Enemy enemy)
    {
        _killsToOpen--;

        if (_killsToOpen > 0)
        {
            return;
        }
        
        _particles.Visible = true;
        _canTeleport = true;
    }

    
    private void OnAreaEntered(Area3D area3D)
    {
        if (_canTeleport == false)
        {
            return;
            
        }
        
        area3D.GetParent<Player>().GlobalPosition = _teleportDestination.GlobalPosition;
    }
}
