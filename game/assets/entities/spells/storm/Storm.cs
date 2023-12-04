using Godot;

public partial class Storm : TargetSpell
{
    [Export] private PackedScene _miniStormSpell;
    [Export] private uint _damage;
    
    public override void UpgradeByLevel(uint level)
    {
        if (level == 1)
        {
            Speed = 7.5f;
        }
        else if (level == 2)
        {
            Speed = 11.5f;
        }
        else
        {
            _damage = 2;
        }
    }
    
    protected override void OnAreaEnteredInternal(Area3D area3D)
    {
        MiniStorm miniSpell = _miniStormSpell.Instantiate() as MiniStorm;

        if (miniSpell == null)
        {
            return;
        }
        
        GetTree().Root.AddChild(miniSpell);
        miniSpell.GlobalPosition = GlobalPosition;
        miniSpell.Cast();
        
        HitboxComponent hitboxComponent = (HitboxComponent)area3D;
        hitboxComponent.Hit(_damage);
        
        QueueFree();
    }
}