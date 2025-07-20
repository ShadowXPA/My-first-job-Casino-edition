using Godot;
using ProjectGJ.Scripts;
using System;

namespace ProjectGJ.Ui.Menus;

public partial class MainMenu : PanelContainer
{
	[Export]
	public Control? Tutorial;
	[Export]
	public Control? Credits;

	public override void _Ready()
	{
		var play = GetNode<Button>("%Play");
		var tutorial = GetNode<Button>("%Tutorial");
		var credits = GetNode<Button>("%Credits");
		var quit = GetNode<Button>("%Quit");

		var sceneManager = SceneManager.Instance!;

		play.Pressed += sceneManager.Play;
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
}
