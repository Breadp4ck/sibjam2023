using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// This class decides how and what to set in 'SpellCaster' 
/// </summary>
public partial class SpellPresenter : Node3D
{
	[Export] private SpellSelectType _spellSelectType;

	public SpellType ChosenSpellType => _chosenSpellType;
	private SpellType _chosenSpellType;

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
				
			_chosenSpellType = ((SpellType)i);
			break;
		}
	}
}

public enum SpellSelectType
{
	Keyboard,
	VoiceControl,
}
