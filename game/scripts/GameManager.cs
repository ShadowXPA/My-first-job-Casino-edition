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
    [ExportGroup("UI")]
    [Export]
    public Hud? Hud;
    [Export]
    public ShopMenu? ShopMenu;
    [Export]
    public StatueMenu? StatueMenu;

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
        SignalBus.PlayerBuyingCasinoGame += OnPlayerBuyingCasinoGame;
        SignalBus.PlayerHiringWorker += OnPlayerHiringWorker;
        SignalBus.PlayerBuyingStatue += OnPlayerBuyingStatue;
        SignalBus.PlayerBoughtCasinoGame += OnPlayerBoughtCasinoGame;
        SignalBus.PlayerHiredWorker += OnPlayerHiredWorker;
        SignalBus.PlayerBoughtStatue += OnPlayerBoughtStatue;

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

        // TODO: every 30 days remove money from salary
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

        // TODO: don't remove money when HIRING, only after 30 days or when firing the worker...
        // var transaction = new Transaction();

        // transaction.AmountBeforeTransaction = _gameData.Money;
        // _gameData.Money += -worker.FinalPrice;
        // transaction.AmountAfterTransaction = _gameData.Money;
        // transaction.TransactionAmount = -worker.FinalPrice;

        SignalBus.BroadcastPlayerHiredWorker(worker);
        // SignalBus.BroadcastPlayerMoneyTransaction(transaction);
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
}
