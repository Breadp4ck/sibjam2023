using Godot;
using System;
using System.Threading.Tasks;

public partial class EnemySpawner : Node3D
{
    [Export] private PackedScene _enemyScene;
    
    [Export] private bool _autoSpawn;
    [Export] private uint _maxEnemies;
    [Export] private float _spawnIntervalSeconds;

    [Export] private string _pathToPlayerInScene; // Path to player RELATIVE to this node!!! SANYAAAAAAAAAAAAAAAAAAAAAAA

    public override async void _Ready()
    {
        if (_autoSpawn == false)
        {
            return;
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
            Spawn(_enemyScene, GetParent().GetNode<Player>(_pathToPlayerInScene));
            enemiesSpawned++;

            await Task.Delay((int)(_spawnIntervalSeconds * 1000));
        }
        
        QueueFree();
    }
}
