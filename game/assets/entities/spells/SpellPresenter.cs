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
				
			Spell spell = GetSpell((SpellType)i);
			_spellCaster.SetSpell(spell);
				
			break;
		}
	}
	
	private Spell GetSpell(SpellType spellType)
	{
		Node spellNode;
		Spell spell;
		
		switch (spellType)
		{
			case SpellType.Cum:
				spellNode = _spellObject[(int)spellType].Instantiate();
				AoESpell cumSpell = (AoESpell)spellNode.GetScript();
				cumSpell.SetDirection(_spellCaster.LookDirection);
				spell = cumSpell;
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
