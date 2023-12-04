using Godot;
using System;

public partial class CassettePlayer : Node
{
	[Export] private AudioStreamPlayer3D _audioPlayer;

	private void OnAreaEntered(Area3D area3D)
	{
		if (_audioPlayer.Playing == true)
		{
			return;
		}
		
		Player player = area3D.GetParent<Player>();
		
		if (player == null)
		{
			return;
		}
		
		_audioPlayer?.Play();
	}
}
