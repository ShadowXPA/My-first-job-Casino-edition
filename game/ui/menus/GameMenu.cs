using Godot;
using ProjectGJ.Scripts;
using System;

namespace ProjectGJ.Ui.Menus;

public partial class GameMenu : PanelContainer
{
	public override void _Ready()
	{
		var mainMenu = GetNode<Button>("%MainMenu");
		var quit = GetNode<Button>("%Quit");

		var sceneManager = SceneManager.Instance!;

		mainMenu.Pressed += sceneManager.MainMenu;
		quit.Pressed += sceneManager.Quit;
	}

	public override void _UnhandledKeyInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))
		{
			Visible = !Visible;
		}
	}
}
