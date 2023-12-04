using Godot;
using System;

public partial class EnemySpawner : Node3D
{
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
}
