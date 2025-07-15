using System;
using Godot;

public static class Utils
{
    public static Vector2 GetCardinalDirection(Vector2 input)
    {
        if (input == Vector2.Zero) return Vector2.Zero;
        return Mathf.Abs(input.X) > Mathf.Abs(input.Y)
            ? (input.X > 0 ? Vector2.Right : Vector2.Left)
            : (input.Y > 0 ? Vector2.Down : Vector2.Up);
    }

    public static string GetTimeFromElapsedTime(int time)
    {
        var hours = time / 60 % 24;
        var minutes = time % 60;
        return $"{hours:d2}:{minutes:d2}";
    }

    public static string GetDayFromElapsedTime(int time)
    {
        return $"{time / 1440}";
    }

    public static Button CreateActionButton(string text, Action action, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left)
    {
        var button = new Button();
        button.Text = text;
        button.Alignment = horizontalAlignment;
        button.Pressed += action;
        return button;
    }
}
