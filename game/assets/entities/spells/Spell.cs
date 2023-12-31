using Godot;

/// <summary>
/// <para>Global spell.</para>>
/// <para>All spells inherit from this class.</para>
/// <para>Impacts all level</para>
/// </summary>
public abstract partial class Spell : Node3D
{
	public SpellType SpellType => Type; 
	[Export] protected SpellType Type;
	
	[Export] protected string SpellName;
	[Export] protected string Description;

	[Export] protected float Duration;
	[Export] public int ManaCost;

	public abstract void Cast();

	public virtual void UpgradeByLevel(uint level) { }
	
	public override string ToString()
	{
		return $"Type: {Type}. Name: {SpellName}. Description: {Description}.";
	}
}
