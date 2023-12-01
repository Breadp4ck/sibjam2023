using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// This class decides how and what to set in 'SpellCaster' 
/// </summary>
public partial class SpellPresenter : Node3D
{
	[Export] private SpellSelectType _spellSelectType;
	[Export] private SpellCaster _spellCaster;

	// Objects MUST be in the same order as in 'SpellType' enum!
	[Export] private PackedScene[] _spellObject;

	private const string SelectSpellSignature = "select_spell_";

	public override void _Input(InputEvent inputEvent)
	{
		if (_spellSelectType != SpellSelectType.Keyboard)
		{
			return;
		}

		for (var i = 0; i < Enum.GetNames(typeof(SpellSelectType)).Length; i++)
		{
			string selectSpellActionName = SelectSpellSignature + (i+1);
				
			if (inputEvent.IsActionPressed(selectSpellActionName) == false)
			{
				continue;
			}
				
			SetupSpell((SpellType)i);
			break;
		}
	}
	
	public void SetupSpell(SpellType spellType)
	{
		Spell spell = GetSpell(spellType);
		_spellCaster.SetSpell(spell);
	}
	
	private Spell GetSpell(SpellType spellType)
	{
		Node spellNode = _spellObject[(int)spellType].Instantiate();
		AddChild(spellNode);
		Spell spell;
		
		switch (spellType)
		{
			case SpellType.Cum:
				AoESpell cumSpell = (AoESpell)spellNode;
				cumSpell.SetDirection(_spellCaster.LookDirection);
				spell = cumSpell;
				break;
			
			case SpellType.TimeSlow:
				Spell timeSlowSpell = (Spell)spellNode;
				spell = timeSlowSpell;
				break;
				
			default:
				throw new ArgumentOutOfRangeException(nameof(spellType), spellType, null);
		}

		return spell;
	}
}

public enum SpellSelectType
{
	Keyboard,
	VoiceControl,
}
