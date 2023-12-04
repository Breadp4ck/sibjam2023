using Godot;

public partial class Fireball : AoESpell
{
	[Export] private double _damage;

	public override void UpgradeByLevel(uint level)
	{
		if (level == 1)
		{
			Speed = 20f;
		}
		else if (level == 2)
		{
			Speed = 30f;
		}
		else
		{
			_damage = 2;
		}
	}
	
	protected override void OnAreaEnteredInternal(Area3D area3D)
	{
		HitboxComponent hitboxComponent = (HitboxComponent)area3D;
		hitboxComponent.Hit(_damage);
	}
}
