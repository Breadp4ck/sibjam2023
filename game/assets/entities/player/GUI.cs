using Godot;
using System;
using Godot.Collections;

public partial class GUI : Control
{
	[Export] private Texture2D[] _spellTextures;
	[Export] private TextureRect _image;
	[Export] private ProgressBar _hpBar;
	[Export] private ProgressBar _manaBar;
	[Export] private HitboxComponent _playerHitbox;
	[Export] private SpellCaster _spellCaster;
	
	private Dictionary<SpellType, Texture2D> _dictionary = new ();

	public override void _Ready()
	{
		SpellPresenter.SpellTypeChangedEvent += OnSpellTypeChanged;
		for (var i = 0; i < _spellTextures.Length; i++)
		{
			var texture = _spellTextures[i];
			_dictionary.Add((SpellType)i, texture);
		}

		_playerHitbox.HpChangedEvent += OnHpChanged;
		_spellCaster.ManaChangedEvent += OnManaChanged;
	}

	private void OnSpellTypeChanged(SpellType spellType)
	{
		if (_dictionary.TryGetValue(spellType, out Texture2D textureRect) == false)
		{
			return;
		}

		_image.Texture = textureRect;
	}

	private void OnHpChanged(double health)
	{
		_hpBar.Value = 100 - health;
	}
	
	private void OnManaChanged(int mana)
	{
		GD.Print("PIZDA");
		_manaBar.Value = 100 - mana;
	}
}
