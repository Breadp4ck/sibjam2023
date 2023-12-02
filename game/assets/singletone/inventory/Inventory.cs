using System.Collections.Generic;
using Godot;

public static class Inventory
{
    private static readonly Dictionary<SpellType, uint> SpellsLevel = new();

    public static bool HasSpell(SpellType spellType)
    {
        return SpellsLevel.ContainsKey(spellType);
    }
    
    public static void AddSpell(SpellType spellType)
    {
        if (SpellsLevel.ContainsKey(spellType))
        {
            SpellsLevel[spellType]++;
            GD.Print("Increased level of " + spellType + " to " + SpellsLevel[spellType]);
        }
        else
        {
            SpellsLevel.Add(spellType, 1);
            GD.Print("Added " + spellType + " to inventory.");
        }
    }
}