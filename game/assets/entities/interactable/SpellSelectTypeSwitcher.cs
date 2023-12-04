using Godot;
using System;

public partial class SpellSelectTypeSwitcher : Area3D
{
    [Export] private SpellSelectType _spellSelectType;
    [Export] private SpellPresenter _spellPresenter;

    public override void _Ready()
    {
        _spellPresenter = GetParent().GetNode<SpellPresenter>("../Player/SpellPresenter");
    }

    public override void _Process(double delta)
    {
        if (InteractRaycast.TypeSwitcher != this)
        {
            return;
        }
        
        if (Input.IsActionJustPressed("interact"))
        {
            Switch();
        }
    }

    private void Switch()
    {
        _spellPresenter.SetSpellSelectType(_spellSelectType);
    }
}
