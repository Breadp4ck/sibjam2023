using Godot;
using System;
using Godot.Collections;

public partial class GUI : Control
{

	[Export] private Texture2D[] _spellTextures;
	[Export] private TextureRect _image;
	
	private Dictionary<SpellType, Texture2D> _dictionary = new ();

	public override void _Ready()
	{
		SpellPresenter.SpellTypeChangedEvent += OnSpellTypeChanged;
		for (var i = 0; i < _spellTextures.Length; i++)
		{
			var texture = _spellTextures[i];
			_dictionary.Add((SpellType)i, texture);
		}
	}

	private void OnSpellTypeChanged(SpellType spellType)
	{
		if (_dictionary.TryGetValue(spellType, out Texture2D textureRect) == false)
		{
			return;
		}

		_image.Texture = textureRect;
	}
}