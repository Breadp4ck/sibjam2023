using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public partial class Spurn : AoESpell
{
    [Export] private float _spurnForce;
    
    protected override async void OnAreaEnteredInternal(Area3D area3D)
    {
        Enemy enemy = area3D.GetParent<Enemy>();
        
        if (enemy == null)
        {
            GD.Print("Failed to get Enemy.");
            return;
        }

        Vector3 kek = enemy.GlobalPosition - GlobalPosition;
        
        kek.Y = 0.5f;
        kek = kek.Normalized();
        enemy.ApplyImpulse(kek * _spurnForce);
    }
}
