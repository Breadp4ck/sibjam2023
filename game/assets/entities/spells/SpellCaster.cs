using Godot;
using System;

public partial class SpellCaster : Node3D
{
	[Export] private Spell _chosenSpell;

	public Vector3 LookDirection => _lookDirection;
	[Export] private Vector3 _lookDirection;
	
	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed("cast_spell"))
		{
			CastSpell();
		}
	}

	public void SetSpell(Spell spell)
	{
		_chosenSpell = spell;
		GD.Print($"{spell} set!");
	}
	
	// Call me outside of this class to cast a spell.
	public void CastSpell()
	{
		if (_chosenSpell == null)
		{
			GD.Print("Spell is NULL. Choose a spell first!");
			return;
		}
		
		_chosenSpell.Cast();
		GD.Print($"Casted {_chosenSpell}!");
	}
}
