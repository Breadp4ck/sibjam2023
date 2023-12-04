using Godot;
using System;

/// <summary>
/// This class decides how and what to set in 'SpellCaster' 
/// </summary>
public partial class SpellPresenter : Node3D
{
	public static event Action<SpellType> SpellTypeChangedEvent;
	
	public SpellSelectType SpellSelectType => _spellSelectType;
	[Export] private SpellSelectType _spellSelectType;

	public SpellType? ChosenSpellType => _chosenSpellType;
	private SpellType? _chosenSpellType;

	private const string SelectSpellSignature = "select_spell_";
	private const uint PlayerOnlySpellTypesCount = 5;
	
	public override void _Input(InputEvent inputEvent)
	{
		if (_spellSelectType != SpellSelectType.Keyboard)
		{
			return;
		}

		SelectViaKeyboard(inputEvent);
	}
	
	public void SetSpellSelectType(SpellSelectType spellSelectType)
	{
		_spellSelectType = spellSelectType;
		GD.Print($"SpellSelectType set to {_spellSelectType}");
	}
	
	public bool TryChooseSpell(SpellType spellType)
	{
		if (Inventory.HasSpell(spellType) == false)
		{
			return false;
		}
		
		_chosenSpellType = spellType;
		SpellTypeChangedEvent?.Invoke(spellType);
		GD.Print($"Spell {_chosenSpellType} selected!");
		return true;
	}
	
	private void SelectViaKeyboard(InputEvent inputEvent)
	{
		for (var i = 0; i < PlayerOnlySpellTypesCount; i++)
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
}

public enum SpellSelectType
{
	Keyboard,
	VoiceControl,
}
