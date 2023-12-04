class_name EndGameArea
extends Node

func _on_area_entered(area: Area3D) -> void:
	var end_game_screen = get_tree().get_first_node_in_group("EndGameScreen")
	end_game_screen.visible = true
