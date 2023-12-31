using Godot;
using System;

public partial class SpellCaster : Node3D
{
	public event Action<int> ManaChangedEvent;
	
	[Export] private Camera3D _camera;

	[Export] private SpellPresenter _spellPresenter;

	[Export] private int _mana = 100;

	// Objects MUST be in the same order as in 'SpellType' enum!
	[Export] private PackedScene[] _spellObject;
	
	private Vector3 _lookDirection;
	
	private Enemy _target; // Capture me using last raycast hit.

	public override void _Ready()
	{
		var manaPerKill = 15;
		Enemy.DieEvent += () =>
		{
			_mana += manaPerKill;
			ManaChangedEvent?.Invoke(_mana);
		};
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed("cast"))
		{
			CastSpell();
		}
	}

	public override void _Process(double delta)
	{
		if (_target != null && IsInstanceValid(_target) == false)
		{
			_target = null;
		}
		
		_lookDirection = -_camera.Transform.Basis.Z;
	}

	public void SetTarget(Enemy target)
	{
		_target = target;
	}
	
	// Call me outside of this class to cast a spell.
	private void CastSpell()
	{
		if (_spellPresenter.ChosenSpellType == null)
		{
			return;
		}
		
		Spell spell = GetSpell(_spellPresenter.ChosenSpellType);
		
		if (spell == null)
		{
			GD.Print("Will not cast. Spell is NULL.");
			return;
		}

		if (_mana < spell.ManaCost)
		{
			spell.QueueFree();
			GD.Print("Not enough mana for cast");
		}
		else
		{
			spell.Cast();
			_mana -= spell.ManaCost;
			ManaChangedEvent?.Invoke(_mana);
			GD.Print($"Casted {spell}!");
		}
	}
	
	private Spell GetSpell(SpellType? spellType)
	{
		Node3D spellNode = (Node3D)_spellObject[(int)spellType].Instantiate();
		GetTree().Root.AddChild(spellNode);
		spellNode.GlobalPosition = GlobalPosition;

		Spell spell;
		
		switch (spellType)
		{
			case SpellType.TimeTwister:
				if (TimeTwisterSpell.IsCasting == true)
				{
					GD.Print("Can`t use 'TimeTwister' spell while it is casting!");
					spellNode.QueueFree();
					return null;
				}
				
				Spell timeSlowSpell = (Spell)spellNode;
				spell = timeSlowSpell;
				break;
			
			case SpellType.SlowMob:
				if (_target == null)
				{
					GD.Print("Can`t use 'SlowMob' spell without a target!");
					spellNode.QueueFree();
					return null;
				}
				
				TargetSpell slowMobSpell = (TargetSpell)spellNode;
				slowMobSpell.SetTarget(_target);
				spell = slowMobSpell;
				break;

			case SpellType.Storm:
				if (_target == null)
				{
					GD.Print("Can`t use 'Storm' spell without a target!");
					spellNode.QueueFree();
					return null;
				}
				
				TargetSpell stormSpell = (TargetSpell)spellNode;
				stormSpell.SetTarget(_target);
				spell = stormSpell;
				break;

			case SpellType.Fireball:
				AoESpell fireball = (AoESpell)spellNode;
				fireball.SetDirection(_lookDirection);
				spell = fireball;
				break;
				
			case SpellType.Spurn:
				AoESpell spurn = (AoESpell)spellNode;
				spurn.SetDirection(_lookDirection);
				spell = spurn;
				break;
				
			default:
				spellNode.QueueFree();
				throw new ArgumentException(spellType + " is not supported!");
		}

		spell.UpgradeByLevel(Inventory.GetLevel(spell.SpellType));
		return spell;
	}
}
