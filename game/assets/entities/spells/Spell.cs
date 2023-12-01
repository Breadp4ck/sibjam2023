using Godot;
using System;

/// <summary>
/// <para>Global spell.</para>>
/// <para>All spells inherit from this class.</para>
/// <para>Impacts all level</para>
/// </summary>
public partial class Spell : Node3D
{
	public SpellType SpellType => Type; 
	[Export] protected SpellType Type;
	
	[Export] protected string SpellName;
	[Export] protected string Description;

	[Export] protected float Duration;
	[Export] protected uint ManaCost;

	public virtual void Cast() { }
	
	public override string ToString()
	{
		return $"Type: {Type}. Name: {SpellName}. Description: {Description}.";
	}
}
