using Godot;
using System;
using Godot.Collections;

public partial class Speech : Node
{
    [Export] private SpellPresenter _spellPresenter;
    
    // Must match spells.dict file.
    private Dictionary<string, SpellType> _spellTypeBySpeech = new()
    {
        {"fire", SpellType.TimeTwister},
        {"slow", SpellType.SlowMob},
        {"storm", SpellType.Storm},
        {"fire", SpellType.Fireball},
        {"spurn", SpellType.Spurn},
    }; 
    
    // From Signal
    private void OnSpeechParsed(string text)
    {
        GD.Print("Catch " + text);

        if (text == string.Empty)
        {
            return;
        }
        
        if (_spellTypeBySpeech.TryGetValue(text, out SpellType value) == true)
        {
            _spellPresenter.TryChooseSpell(value);
            return;
        }
        
        
        GD.Print("ERROR: No spell for " + text);
    }
}
