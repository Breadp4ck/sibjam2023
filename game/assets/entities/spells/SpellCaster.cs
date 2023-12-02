using Godot;
using System;

public partial class SpellCaster : Node3D
{
	[Export] private Camera3D _camera;

	[Export] private SpellPresenter _spellPresenter;
	
	// Objects MUST be in the same order as in 'SpellType' enum!
	[Export] private PackedScene[] _spellObject;
	
	private Spell _chosenSpell;
	private Vector3 _lookDirection;

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed("cast"))
		{
			CastSpell();
		}
	}

	public override void _Process(double delta)
	{
		_lookDirection = -_camera.Transform.Basis.Z;
	}

	private void SetSpell(Spell spell)
	{
		_chosenSpell = spell;
		GD.Print($"{spell} set!");
	}
	
	// Call me outside of this class to cast a spell.
	private void CastSpell()
	{
		Spell spell = GetSpell(_spellPresenter.ChosenSpellType);
		
		if (spell == null)
		{
			GD.Print("Spell is NULL. Choose a spell first!");
			return;
		}

		spell.Cast();
		GD.Print($"Casted {spell}!");
	}
	
	private Spell GetSpell(SpellType spellType)
	{
		Node3D spellNode = (Node3D)_spellObject[(int)spellType].Instantiate();
		GetTree().Root.AddChild(spellNode);
		spellNode.GlobalPosition = GlobalPosition;

		Spell spell;
		
		switch (spellType)
		{
			case SpellType.Cum:
				AoESpell cumSpell = (AoESpell)spellNode;
				cumSpell.SetDirection(_lookDirection);
				spell = cumSpell;
				break;
			
			case SpellType.TimeSlow:
				Spell timeSlowSpell = (Spell)spellNode;
				spell = timeSlowSpell;
				break;
				
			default:
				return null;
		}

		return spell;
	}
}
