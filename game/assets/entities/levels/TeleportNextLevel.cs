using Godot;
using System;
using System.Threading.Tasks;

public partial class TeleportNextLevel : Area3D
{
    [Export] private Vector3 _teleportDestination;
    [Export] private uint _killsToOpen;

    [Export] private Node3D _particles;
    
    private bool _canTeleport;
    
    public override async void _Ready()
    {
        Player player = (Player)GetTree().Root.GetChild(0).GetChild(0);
        while (true)
        {
            if (player.GlobalPosition.DistanceTo(GlobalPosition) < 50)
            {
                break;
            }

            await Task.Delay(1000);
        }
        
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
        
        area3D.GetParent<Player>().GlobalPosition += _teleportDestination;
    }
}
