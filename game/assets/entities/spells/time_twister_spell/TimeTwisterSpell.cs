using Godot;
using System;
using System.Threading.Tasks;

public partial class TimeTwisterSpell : Spell
{
    public static bool IsCasting => _isCasting;
    private static bool _isCasting;
    
    [Export] private float _playerNewTimescale; // e.g. 1.7f => 170% of the original timescale will be used.
    [Export] private float _enemyNewTimescale; // e.g. 0.7f => 70% of the original timescale will be used.
    
    public override async void Cast()
    {
        if (_isCasting == true)
        {
            return;
        }

        _isCasting = true;
        
        Timescale.SetPlayerTimescale(_playerNewTimescale);
        Timescale.SetEnemyTimescale(_enemyNewTimescale);

        GD.Print($"Player Timescale is now set to {Timescale.Player}");
        GD.Print($"Enemy Timescale is now set to {Timescale.Enemy}");

        await Task.Delay((int)(Duration * 1000)); // Wait for the spell to end.
        
        Timescale.SetPlayerTimescale(1.0f);
        Timescale.SetEnemyTimescale(1.0f);
        
        GD.Print($"Player Timescale is now set to {Timescale.Player}");
        GD.Print($"Enemy Timescale is now set to {Timescale.Enemy}");

        _isCasting = false;
        
        QueueFree();
    }
}
