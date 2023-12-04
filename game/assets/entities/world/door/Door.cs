using Godot;
using System;
using System.Linq;

public partial class Door : Node
{
    [Export] private uint _killsToOpen;
    [Export] private string[] _namesOfEnemiesToKill;
    
    public override void _Ready()
    {
        HealthComponent.Died += OnEnemyDied;
    }

    private void OnEnemyDied(Enemy enemy)
    {
        if (_namesOfEnemiesToKill.Contains(enemy.GetType().ToString()) == false)
        {
            return;
        }
        
        _killsToOpen--;

        if (_killsToOpen <= 0)
        {
            Open();
        }
    }

    private void Open()
    {
        GD.Print("Door is opened!");
        throw new NotImplementedException();
    }
}
