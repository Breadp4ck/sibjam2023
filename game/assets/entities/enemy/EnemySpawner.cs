using Godot;
using System;
using System.Threading.Tasks;

public partial class EnemySpawner : Node3D
{
    [Export] private PackedScene _enemyScene;
    
    [Export] private bool _autoSpawn;
    [Export] private uint _maxEnemies;
    [Export] private float _spawnIntervalSeconds;

    [Export] private string _pathToPlayerInScene;

    private Player _player;
    
    public override async void _Ready()
    {
        if (_autoSpawn == false)
        {
            return;
        }

        _player = (Player)GetTree().Root.GetChild(0).GetChild(0);

        while (true)
        {
            if (_player.GlobalPosition.DistanceTo(GlobalPosition) < 50)
            {
                GD.Print("PZIDA");
                break;
            }

            await Task.Delay(1000);
        }
        
        await Task.Delay((int)(_spawnIntervalSeconds * 1000));
        AutoSpawn();
    }

    public void Spawn(PackedScene enemyScene, Player target)
    {
        Enemy enemy = enemyScene.Instantiate() as Enemy;

        if (enemy == null)
        {
            return;
        }
        
        GetTree().Root.AddChild(enemy);
        enemy.GlobalPosition = GlobalPosition;
        enemy.SetTarget(target);
    }

    private async void AutoSpawn()
    {
        uint enemiesSpawned = 0;
        while (enemiesSpawned < _maxEnemies)
        {
            Spawn(_enemyScene, _player);
            enemiesSpawned++;

            await Task.Delay((int)(_spawnIntervalSeconds * 1000));
        }
        
        QueueFree();
    }
}
