using Godot;
using System;

/// <summary>
/// This class decides how and what to set in 'SpellCaster' 
/// </summary>
public partial class SpellPresenter : Node3D
{
	[Export] private SpellSelectType _spellSelectType;

	public SpellType? ChosenSpellType => _chosenSpellType;
	private SpellType? _chosenSpellType;

	private const string SelectSpellSignature = "select_spell_";

	public override void _Input(InputEvent inputEvent)
	{
		if (_spellSelectType != SpellSelectType.Keyboard)
		{
			return;
		}

		for (var i = 0; i < Enum.GetNames(typeof(SpellType)).Length; i++)
		{
			string selectSpellActionName = SelectSpellSignature + (i+1);
				
			if (inputEvent.IsActionPressed(selectSpellActionName) == false)
			{
				continue;
			}
			
			SpellType wantedSpellType = (SpellType)i;

			if (TryChooseSpell(wantedSpellType) == false)
			{
				GD.Print($"You don't have {wantedSpellType} spell!");
			}
			
			break;
		}
	}
	
	private bool TryChooseSpell(SpellType spellType)
	{
		if (Inventory.HasSpell(spellType) == false)
		{
			return false;
		}
		
		_chosenSpellType = spellType;
		GD.Print($"Spell {_chosenSpellType} selected!");
		return true;
	}
}

public enum SpellSelectType
{
	Keyboard,
	VoiceControl,
}
