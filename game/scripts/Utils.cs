using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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

    public static string? GetAvailableCharacterResource(Gender gender, Profession profession, int? index = null, params string[] removeResource)
    {
        var availableCharacters = GameItems.AvailableCharacters[(gender, profession)].Where(s => !removeResource.Contains($"{Constants.CHARACTER_RESOURCE_BASE_PATH}/{s}")).ToList();
        var numOfAvailableCharacters = availableCharacters.Count;

        if (numOfAvailableCharacters == 0 || index < 0 || index > numOfAvailableCharacters) return null;

        var i = index ?? GD.RandRange(0, numOfAvailableCharacters - 1);
        return $"{Constants.CHARACTER_RESOURCE_BASE_PATH}/{availableCharacters[i]}";
    }

    public static string GenerateRandomName(Gender gender)
    {
        return $"{GameItems.Names[gender][GD.RandRange(0, GameItems.Names[gender].Count - 1)]} {GameItems.Surnames[GD.RandRange(0, GameItems.Surnames.Count - 1)]}";
    }

    public static CustomerItem GenerateRandomCustomer(string playerResource, Gender? gender = null, CustomerType? customerType = null, float cheaterMultiplier = 1.0f, float addictMultiplier = 1.0f, float averageTimeMultiplier = 1.0f)
    {
        var genderRandom = GD.Randf();
        var customerTypeRandom = GD.Randf();
        var cheaterRate = Constants.RANDOM_CHEATER_RATE * cheaterMultiplier;
        var addictRate = Constants.RANDOM_ADDICT_RATE * addictMultiplier;
        var g = gender ?? (genderRandom < Constants.RANDOM_GENDER_RATE ? Gender.Male : Gender.Female);
        var ct = customerType ?? (customerTypeRandom < cheaterRate ? CustomerType.Cheater : (customerTypeRandom < (cheaterRate + addictRate)) ? CustomerType.Addict : CustomerType.Normal);
        var customerAvgTime = GenerateCustomerAverageTime(ct, averageTimeMultiplier);
        var activities = GenerateCustomerActivities(customerAvgTime);

        return new CustomerItem()
        {
            Name = GenerateRandomName(g),
            CustomerType = ct,
            Resource = GetAvailableCharacterResource(g, Profession.Customer, removeResource: playerResource),
            BonusWinRate = GenerateCustomerBonusWinRate(ct),
            Activities = activities
        };
    }

    public static float GenerateCustomerBonusWinRate(CustomerType customerType)
    {
        return customerType switch
        {
            CustomerType.Normal => (float)GD.RandRange(Constants.CustomerBonusWinRate.MIN_NORMAL, Constants.CustomerBonusWinRate.MAX_NORMAL),
            CustomerType.Cheater => (float)GD.RandRange(Constants.CustomerBonusWinRate.MIN_CHEATER, Constants.CustomerBonusWinRate.MAX_CHEATER),
            CustomerType.Addict => (float)GD.RandRange(Constants.CustomerBonusWinRate.MIN_ADDICT, Constants.CustomerBonusWinRate.MAX_ADDICT),
            _ => 0
        };
    }

    public static int GenerateCustomerAverageTime(CustomerType customerType, float avgTimeMultiplier = 1.0f)
    {
        return customerType switch
        {
            CustomerType.Normal => GD.RandRange(Mathf.FloorToInt(Constants.CustomerAverageTime.MIN_NORMAL * avgTimeMultiplier),
                Mathf.FloorToInt(Constants.CustomerAverageTime.MAX_NORMAL * avgTimeMultiplier)),
            CustomerType.Cheater => GD.RandRange(Mathf.FloorToInt(Constants.CustomerAverageTime.MIN_CHEATER * avgTimeMultiplier),
                Mathf.FloorToInt(Constants.CustomerAverageTime.MAX_CHEATER * avgTimeMultiplier)),
            CustomerType.Addict => GD.RandRange(Mathf.FloorToInt(Constants.CustomerAverageTime.MIN_ADDICT * avgTimeMultiplier),
                Mathf.FloorToInt(Constants.CustomerAverageTime.MAX_ADDICT * avgTimeMultiplier)),
            _ => 0
        };
    }

    public static List<CustomerActivity> GenerateCustomerActivities(int averageTime, int maxActivities = 5)
    {
        int numActivities = 0;
        int timeLeft = averageTime;
        List<CustomerActivity> activities = [];

        while (timeLeft != 0)
        {
            var activity = new CustomerActivity();

            activity.Activity = (ActivityType)GD.RandRange((int)ActivityType.Bar, (int)ActivityType.Roulette);

            if ((timeLeft - Constants.MIN_ACTIVITY_TIME) < Constants.MIN_ACTIVITY_TIME || numActivities == maxActivities - 1)
            {
                activity.TimeLeft = timeLeft;
                timeLeft = 0;
            }
            else
            {
                var activityTime = GD.RandRange(Constants.MIN_ACTIVITY_TIME, timeLeft - Constants.MIN_ACTIVITY_TIME);
                activity.TimeLeft = activityTime;
                timeLeft -= activityTime;
            }

            activities.Add(activity);
            numActivities++;
        }

        activities.Add(new CustomerActivity()
        {
            Activity = ActivityType.Home
        });
        return activities;
    }

    public static WorkerItem GenerateRandomWorker(Gender? gender = null, Profession? profession = null)
    {
        var genderRandom = GD.Randf();
        var professionRandom = GD.Randf();
        var salary = GD.RandRange(Constants.MIN_SALARY, Constants.MAX_SALARY);
        var g = gender ?? (genderRandom < Constants.RANDOM_GENDER_RATE ? Gender.Male : Gender.Female);
        var p = profession ?? (professionRandom < Constants.RANDOM_PROFESSION_RATE ? Profession.Security : professionRandom < (Constants.RANDOM_PROFESSION_RATE * 2) ? Profession.Bartender : Profession.Dealer);

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
