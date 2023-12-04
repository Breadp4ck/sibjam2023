using System.Collections.Generic;
using Godot;

public static class Inventory
{
    private static readonly Dictionary<SpellType, uint> SpellsLevel = new();

    // ====================> USE ME FOR DEBUG <====================
    static Inventory()
    {
        SpellsLevel = new Dictionary<SpellType, uint>()
        {
            {SpellType.Fireball, 1},
            {SpellType.SlowMob, 1},
            {SpellType.Storm, 1},
            {SpellType.TimeTwister, 1},
            {SpellType.Spurn, 1},
        };
    }
    
    public static bool HasSpell(SpellType spellType)
    {
        return SpellsLevel.ContainsKey(spellType);
    }

    public static uint GetLevel(SpellType spellType)
    {
        return SpellsLevel.ContainsKey(spellType) == false ? 0 : SpellsLevel[spellType];
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