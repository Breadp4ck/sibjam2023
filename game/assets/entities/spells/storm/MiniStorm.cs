using Godot;
using System;

public partial class MiniStorm : AoESpell
{
	[Export] private double _damage;
	
	protected override void OnAreaEnteredInternal(Area3D area3D)
	{
		HitboxComponent hitboxComponent = (HitboxComponent)area3D;
		hitboxComponent.Hit(_damage);
	}
}
