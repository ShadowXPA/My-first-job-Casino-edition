using System;
using Godot;
using ProjectGJ.Scripts.Items;

namespace ProjectGJ.Scripts;

public static class Utils
{
    public static Vector2 ToCardinalDirection(this Vector2 input)
    {
        if (input == Vector2.Zero) return Vector2.Zero;
        return Mathf.Abs(input.X) > Mathf.Abs(input.Y)
            ? (input.X > 0 ? Vector2.Right : Vector2.Left)
            : (input.Y > 0 ? Vector2.Down : Vector2.Up);
    }

    public static (int, int) GetHoursAndMinutes(int elapsedTime)
    {
        var hours = elapsedTime / 60 % 24;
        var minutes = elapsedTime % 60;
        return (hours, minutes);
    }

    public static int GetDays(int elapsedTime)
    {
        return elapsedTime / 1440;
    }

    public static string GetTimeFromElapsedTime(int time)
    {
        var (hours, minutes) = GetHoursAndMinutes(time);
        return $"{hours:d2}:{minutes:d2}";
    }

    public static string GetDayFromElapsedTime(int time)
    {
        return $"{GetDays(time)}";
    }

    public static Button CreateActionButton(string text, Action? action, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left, bool disabled = false)
    {
        var button = new Button();
        button.Text = text;
        if (action is not null)
            button.Pressed += action;
        button.Alignment = horizontalAlignment;
        button.Disabled = disabled;
        return button;
    }

    public static string? GetAvailableCharacterResource(Gender gender, Profession profession, int? index = null)
    {
        var numOfAvailableCharacters = GameItems.AvailableCharacters[(gender, profession)].Count;

        if (numOfAvailableCharacters == 0 || index < 0 || index > numOfAvailableCharacters) return null;

        var i = index ?? GD.RandRange(0, numOfAvailableCharacters - 1);
        return $"{Constants.CHARACTER_RESOURCE_BASE_PATH}/{GameItems.AvailableCharacters[(gender, profession)][i]}";
    }

    public static string GenerateRandomName(Gender gender)
    {
        return $"{GameItems.Names[gender][GD.RandRange(0, GameItems.Names[gender].Count - 1)]} {GameItems.Surnames[GD.RandRange(0, GameItems.Surnames.Count - 1)]}";
    }

    public static WorkerItem GenerateRandomWorker(Gender? gender = null, Profession? profession = null)
    {
        var genderRandom = GD.Randf();
        var professionRandom = GD.Randf();
        var salary = GD.RandRange(Constants.MIN_SALARY, Constants.MAX_SALARY);
        var g = gender ?? (genderRandom < 0.5 ? Gender.Male : Gender.Female);
        var p = profession ?? (professionRandom < 0.33 ? Profession.Security : professionRandom < 0.66 ? Profession.Bartender : Profession.Dealer);

        var name = GenerateRandomName(g);
        var resource = GetAvailableCharacterResource(g, p);
        return new WorkerItem()
        {
            Name = name,
            Description = p.ToString(),
            Profession = p,
            Price = salary,
            Resource = resource,
        };
    }
}
