using System;
using Godot;

public static class Utils
{
    public static Button CreateActionButton(string text, Action action, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left)
    {
        var button = new Button();
        button.Text = text;
        button.Alignment = horizontalAlignment;
        button.Pressed += action;
        return button;
    }
}
