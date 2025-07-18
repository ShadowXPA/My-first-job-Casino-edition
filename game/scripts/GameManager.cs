using Godot;
using ProjectGJ.Characters.Player;
using ProjectGJ.Scripts.Items;
using ProjectGJ.Ui.Hud;
using ProjectGJ.Ui.Menus;
using System;
using System.Collections.Generic;

namespace ProjectGJ.Scripts;

public partial class GameManager : Node
{
    [Export]
    public Timer? GameTimer;
    [Export]
    public Player? Player;
    [Export]
    public Node? CustomerContainer;
    [Export]
    public Node? WorkerContainer;
    [Export]
    public Hud? Hud;

    private GameData _gameData = new();

    public override void _Ready()
    {
        SignalBus.BroadcastGameTimeChanged(_gameData.ElapsedTime);
        Hud?.SetMoney(_gameData.Money);

        if (GameTimer is not null)
        {
            GameTimer.Timeout += OnGameTimerTicked;
        }

        SignalBus.PlayerMoneyTransaction += OnPlayerMoneyTransaction;
        SignalBus.PlayerBuyingCasinoGame += OnPlayerBuyingCasinoGame;
        SignalBus.PlayerHiringWorker += OnPlayerHiringWorker;
        SignalBus.PlayerBuyingStatue += OnPlayerBuyingStatue;
        SignalBus.PlayerBoughtCasinoGame += OnPlayerBoughtCasinoGame;
        SignalBus.PlayerHiredWorker += OnPlayerHiredWorker;
        SignalBus.PlayerBoughtStatue += OnPlayerBoughtStatue;
        SignalBus.PlayerSoldCasinoGame += OnPlayerSoldCasinoGame;
        SignalBus.PlayerFiredWorker += OnPlayerFiredWorker;

        Player?.SetCharacterResource(_gameData.CharacterResource);
    }

    public override void _ExitTree()
    {
        if (GameTimer is not null)
        {
            GameTimer.Timeout -= OnGameTimerTicked;
        }

        SignalBus.PlayerMoneyTransaction -= OnPlayerMoneyTransaction;
        SignalBus.PlayerBuyingCasinoGame -= OnPlayerBuyingCasinoGame;
        SignalBus.PlayerHiringWorker -= OnPlayerHiringWorker;
        SignalBus.PlayerBuyingStatue -= OnPlayerBuyingStatue;
        SignalBus.PlayerBoughtCasinoGame -= OnPlayerBoughtCasinoGame;
        SignalBus.PlayerHiredWorker -= OnPlayerHiredWorker;
        SignalBus.PlayerBoughtStatue -= OnPlayerBoughtStatue;
        SignalBus.PlayerSoldCasinoGame -= OnPlayerSoldCasinoGame;
        SignalBus.PlayerFiredWorker -= OnPlayerFiredWorker;
    }

    public void LoadGame()
    {
    }

    public void SaveGame()
    {
    }

    private void OnGameTimerTicked()
    {
        _gameData.ElapsedTime++;
        SignalBus.BroadcastGameTimeChanged(_gameData.ElapsedTime);

        // TODO: every hour check for broken machines?
        // TODO: every hour try to spawn more customers?
        var (hour, minutes) = Utils.GetHoursAndMinutes(_gameData.ElapsedTime);
        if (hour % 24 == 0 && minutes % 60 == 0)
        {
            foreach (var worker in _gameData.Inventory.Workers)
            {
                worker.DaysWorked++;
            }

            SignalBus.BroadcastNewDay();

            // TODO: every 30 days remove money from salary, maintenance fees (TAXES... yes...)?
            var days = Utils.GetDays(_gameData.ElapsedTime);
            if (days % 30 == 0)
            {
                var salaries = 0;

                foreach (var worker in _gameData.Inventory.Workers)
                {
                    salaries += Mathf.FloorToInt(worker.FinalPrice / 30 * Mathf.Min(worker.DaysWorked, 30));
                }

                var expenses = -salaries;

                var transaction = new Transaction();
                transaction.AmountBeforeTransaction = _gameData.Money;
                _gameData.Money += expenses;
                transaction.AmountAfterTransaction = _gameData.Money;
                transaction.TransactionAmount = expenses;
                SignalBus.BroadcastPlayerMoneyTransaction(transaction);
                SignalBus.BroadcastNotifyPlayer("30 days have passed, you have paid your expenses.");
            }
        }
    }

    private void OnPlayerMoneyTransaction(Transaction transaction)
    {
        _gameData.Transactions.Add(transaction);
    }

    private void OnPlayerBuyingCasinoGame(CasinoGameItem casinoGame)
    {
        if (_gameData.Money < casinoGame.FinalPrice)
        {
            SignalBus.BroadcastNotifyPlayer($"You cannot afford to buy this casino game: {casinoGame.Name}");
            return;
        }

        var transaction = new Transaction();

        transaction.AmountBeforeTransaction = _gameData.Money;
        _gameData.Money += -casinoGame.FinalPrice;
        transaction.AmountAfterTransaction = _gameData.Money;
        transaction.TransactionAmount = -casinoGame.FinalPrice;

        SignalBus.BroadcastPlayerBoughtCasinoGame(casinoGame.Clone());
        SignalBus.BroadcastPlayerMoneyTransaction(transaction);
    }

    private void OnPlayerBoughtCasinoGame(CasinoGameItem casinoGame)
    {
        _gameData.Inventory.CasinoGames.Add(casinoGame);
    }

    private void OnPlayerHiringWorker(WorkerItem worker)
    {
        if (_gameData.Money < worker.FinalPrice)
        {
            SignalBus.BroadcastNotifyPlayer($"You cannot afford to hire this person: {worker.Name}");
            return;
        }

        SignalBus.BroadcastPlayerHiredWorker(worker);
    }

    private void OnPlayerHiredWorker(WorkerItem worker)
    {
        _gameData.Inventory.Workers.Add(worker);
    }

    private void OnPlayerBuyingStatue(StatueItem statue)
    {
        if (_gameData.Money < statue.FinalPrice)
        {
            SignalBus.BroadcastNotifyPlayer($"You cannot afford to buy this statue: {statue.Name}");
            return;
        }

        var transaction = new Transaction();

        transaction.AmountBeforeTransaction = _gameData.Money;
        _gameData.Money += -statue.FinalPrice;
        transaction.AmountAfterTransaction = _gameData.Money;
        transaction.TransactionAmount = -statue.FinalPrice;

        SignalBus.BroadcastPlayerBoughtStatue(statue);
        SignalBus.BroadcastPlayerMoneyTransaction(transaction);
    }

    private void OnPlayerBoughtStatue(StatueItem statue)
    {
        _gameData.Inventory.Statues.Add(statue);
        SignalBus.BroadcastNotifyPlayer($"Got permanent buff: {statue.Description}");
    }

    private void OnPlayerSoldCasinoGame(CasinoGameItem casinoGame)
    {
        var sellValue = Mathf.FloorToInt(casinoGame.FinalPrice * Constants.SELL_PERCENTAGE);

        _gameData.Inventory.CasinoGames.Remove(casinoGame);

        var transaction = new Transaction();

        transaction.AmountBeforeTransaction = _gameData.Money;
        _gameData.Money += sellValue;
        transaction.AmountAfterTransaction = _gameData.Money;
        transaction.TransactionAmount = sellValue;

        SignalBus.BroadcastPlayerMoneyTransaction(transaction);
    }

    private void OnPlayerFiredWorker(WorkerItem worker)
    {
        var sellValue = -Mathf.FloorToInt(worker.FinalPrice / 30 * (worker.DaysWorked % 30));

        worker.Station?.SetWorker(null);
        _gameData.Inventory.Workers.Remove(worker);

        var transaction = new Transaction();

        transaction.AmountBeforeTransaction = _gameData.Money;
        _gameData.Money += sellValue;
        transaction.AmountAfterTransaction = _gameData.Money;
        transaction.TransactionAmount = sellValue;

        SignalBus.BroadcastPlayerMoneyTransaction(transaction);
    }
}
