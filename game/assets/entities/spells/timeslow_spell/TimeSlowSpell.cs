using Godot;
using System;
using System.Threading.Tasks;

public partial class TimeSlowSpell : Spell
{
    [Export] private float _newTimescale; // e.g. 0.7f => 70% of the original timescale will be used.
    
    public override async void Cast()
    {
        Timescale.SetTimescale(_newTimescale);
        
        GD.Print($"Timescale is now set to {Timescale.Scale}");

        await Task.Delay((int)(Duration * 1000));
        
        Timescale.SetTimescale(1.0f);
        GD.Print($"Timescale is now set to {Timescale.Scale}");

        QueueFree();
    }
}
