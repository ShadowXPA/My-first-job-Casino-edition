using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
    [Export]
    public Hud? Hud;
    [Export]
    public Timer? GameTimer;
    [Export]
    public Player? Player;
    [Export]
    public Node? CustomerContainer;
    [Export]
    public Node? WorkerContainer;

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

        if (Player is null) return;

        Player.SetCharacter(_gameData.Character);
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
        // TODO: this should be a signal I guess, that way HUD can set time and day, Game manager or whoever can do checks based on time and day (if it's the 30th day etc...)
        Hud?.SetTimeAndDay(_gameData.ElapsedTime);
        // TODO: remove this...
        Hud?.AddTransaction(GD.Randf() < .5 ? (int)-GD.Randi() : (int)GD.Randi());
    }

    private void OnPlayerMoneyTransaction(Transaction transaction)
    {
        GD.PrintS(transaction.PreviousAmount, transaction.CurrentAmount, transaction.TransactionAmount);
    }
}
