using Godot;
using System;

namespace ProjectGJ.Scripts;

public partial class SceneManager : Node
{
    public static SceneManager? Instance { get; private set; }

    public string? Character;

    public override void _Ready()
    {
        Instance = this;
    }

    public void MainMenu()
    {
        LoadScene("res://scenes/main_menu.tscn");
    }

    public void Play()
    {
        LoadScene("res://scenes/game.tscn");
    }

    public void Quit()
    {
        GetTree().Quit();
    }

    private void LoadScene(string scene)
    {
        GetTree().ChangeSceneToFile(scene);
    }
}
