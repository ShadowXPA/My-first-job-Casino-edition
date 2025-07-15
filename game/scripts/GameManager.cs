using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
    [Export]
    public Hud? Hud;
    [Export]
    public Timer? GameTimer;

    private GameData _gameData = new();

    public override void _Ready()
    {
        Hud?.SetTimeAndDay(_gameData.ElapsedTime);
        Hud?.SetMoney(_gameData.Money);

        if (GameTimer is not null)
        {
            GameTimer.Timeout += OnGameTimerTicked;
        }

        SignalBus.PlayerMoneyTransaction += OnPlayerMoneyTransaction;
    }

    public override void _ExitTree()
    {
        if (GameTimer is not null)
        {
            GameTimer.Timeout -= OnGameTimerTicked;
        }

        SignalBus.PlayerMoneyTransaction -= OnPlayerMoneyTransaction;
    }

    private void OnGameTimerTicked()
    {
        _gameData.ElapsedTime++;
        Hud?.SetTimeAndDay(_gameData.ElapsedTime);
        Hud?.AddTransaction(GD.Randf() < .5 ? (int)-GD.Randi() : (int)GD.Randi());
    }

    private void OnPlayerMoneyTransaction(Transaction transaction)
    {
        GD.PrintS(transaction.PreviousAmount, transaction.CurrentAmount, transaction.TransactionAmount);
    }
}
