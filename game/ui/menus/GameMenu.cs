using Godot;
using ProjectGJ.Scripts;
using System;

namespace ProjectGJ.Ui.Menus;

public partial class GameMenu : PanelContainer
{
	[Export]
	public Control? Tutorial;
	[Export]
	public Control? Credits;

	public override void _Ready()
	{
		var mainMenu = GetNode<Button>("%MainMenu");
		var tutorial = GetNode<Button>("%Tutorial");
		var credits = GetNode<Button>("%Credits");
		var quit = GetNode<Button>("%Quit");

		var sceneManager = SceneManager.Instance!;

		mainMenu.Pressed += sceneManager.MainMenu;
		quit.Pressed += sceneManager.Quit;

		tutorial.Pressed += () =>
		{
			if (Tutorial is not null)
				Tutorial.Visible = true;
		};

		credits.Pressed += () =>
		{
			if (Credits is not null)
				Credits.Visible = true;
		};
	}

	public override void _UnhandledKeyInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))
		{
			Visible = !Visible;

			if (Tutorial is not null)
				Tutorial.Visible = false;

			if (Credits is not null)
				Credits.Visible = false;
		}
	}
}
