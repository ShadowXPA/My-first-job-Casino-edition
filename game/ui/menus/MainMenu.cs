using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectGJ.Scripts;

namespace ProjectGJ.Ui.Menus;

public partial class MainMenu : PanelContainer
{
	[Export]
	public Control? Tutorial;
	[Export]
	public Control? Credits;

	public override void _Ready()
	{
		var charactersList = GetNode<ItemList>("%Characters");

		var play = GetNode<Button>("%Play");
		var tutorial = GetNode<Button>("%Tutorial");
		var credits = GetNode<Button>("%Credits");
		var quit = GetNode<Button>("%Quit");

		var sceneManager = SceneManager.Instance!;

		var availableCharactersKeys = GameItems.AvailableCharacters.Keys.Where(k => k.Item2 == Profession.Player);
		var characters = new Dictionary<int, string>();

		charactersList.ItemSelected += (long idx) =>
		{
			sceneManager.Character = characters[(int)idx];
		};

		foreach (var key in availableCharactersKeys)
		{
			foreach (var character in GameItems.AvailableCharacters[key])
			{
				var idx = charactersList.AddItem($"{character.Replace(".res", "")} ({key.Item1})", Utils.LoadFrameTexture(character));
				characters.Add(idx, character);

				if (character == Constants.DEFAULT_CHARACTER_RESOURCE)
				{
					charactersList.Select(idx);
					sceneManager.Character = character;
				}
			}
		}

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
