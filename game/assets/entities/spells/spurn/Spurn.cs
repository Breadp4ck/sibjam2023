using Godot;

public partial class Spurn : AoESpell
{
    [Export] private float _spurnForce;
    
    public override void UpgradeByLevel(uint level)
    {
        if (level == 1)
        {
            _spurnForce = 10f;
        }
        else if (level == 2)
        {
            _spurnForce = 15f;
        }
        else
        {
            _spurnForce = 30f;
        }
    }
    
    protected override void OnAreaEnteredInternal(Area3D area3D)
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
