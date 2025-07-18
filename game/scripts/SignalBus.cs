using System;
using System.Collections.Generic;
using Godot;
using ProjectGJ.Props.Slots;
using ProjectGJ.Scripts.Items;

namespace ProjectGJ.Scripts;

public static class SignalBus
{
    public static Action<int>? GameTimeChanged;
    public static void BroadcastGameTimeChanged(int time) => GameTimeChanged?.Invoke(time);
    public static Action<List<Button>>? PlayerInteracting;
    public static void BroadcastPlayerInteracting(List<Button> actions) => PlayerInteracting?.Invoke(actions);
    public static Action<List<Button>>? PlayerStoppedInteracting;
    public static void BroadcastPlayerStoppedInteracting(List<Button> actions) => PlayerStoppedInteracting?.Invoke(actions);
    public static Action<Transaction>? PlayerMoneyTransaction;
    public static void BroadcastPlayerMoneyTransaction(Transaction transaction) => PlayerMoneyTransaction?.Invoke(transaction);
    public static Action<Transaction>? TransactionComplete;
    public static void BroadcastTransactionComplete(Transaction transaction) => TransactionComplete?.Invoke(transaction);
    // public static Action<Transaction>? CustomerMoneyTransaction;
    public static Action<StatueItem>? PlayerSelectedStatue;
    public static void BroadcastPlayerSelectedStatue(StatueItem item) => PlayerSelectedStatue?.Invoke(item);
    public static Action<CasinoGameItem>? PlayerBuyingCasinoGame;
    public static void BroadcastPlayerBuyingCasinoGame(CasinoGameItem item) => PlayerBuyingCasinoGame?.Invoke(item);
    public static Action<CasinoGameItem>? PlayerBoughtCasinoGame;
    public static void BroadcastPlayerBoughtCasinoGame(CasinoGameItem item) => PlayerBoughtCasinoGame?.Invoke(item);
    public static Action<WorkerItem>? PlayerHiringWorker;
    public static void BroadcastPlayerHiringWorker(WorkerItem item) => PlayerHiringWorker?.Invoke(item);
    public static Action<WorkerItem>? PlayerHiredWorker;
    public static void BroadcastPlayerHiredWorker(WorkerItem item) => PlayerHiredWorker?.Invoke(item);
    public static Action<StatueItem>? PlayerBuyingStatue;
    public static void BroadcastPlayerBuyingStatue(StatueItem item) => PlayerBuyingStatue?.Invoke(item);
    public static Action<StatueItem>? PlayerBoughtStatue;
    public static void BroadcastPlayerBoughtStatue(StatueItem item) => PlayerBoughtStatue?.Invoke(item);
    public static Action<string>? NotifyPlayer;
    public static void BroadcastNotifyPlayer(string message) => NotifyPlayer?.Invoke(message);
    public static Action<bool>? ShopMenuButtonPressed;
    public static void BroadcastShopMenuButtonPressed(bool open = true) => ShopMenuButtonPressed?.Invoke(open);
    public static Action<bool>? InventoryMenuButtonPressed;
    public static void BroadcastInventoryMenuButtonPressed(bool open = true) => InventoryMenuButtonPressed?.Invoke(open);
    public static Action<bool>? TransactionsMenuButtonPressed;
    public static void BroadcastTransactionsMenuButtonPressed(bool open = true) => TransactionsMenuButtonPressed?.Invoke(open);
    public static Action<bool>? StatueMenuButtonPressed;
    public static void BroadcastStatueMenuButtonPressed(bool open = true) => StatueMenuButtonPressed?.Invoke(open);
    public static Action<CasinoGameItem>? PlayerSoldCasinoGame;
    public static void BroadcastPlayerSoldCasinoGame(CasinoGameItem casinoGame) => PlayerSoldCasinoGame?.Invoke(casinoGame);
    public static Action<WorkerItem>? PlayerFiredWorker;
    public static void BroadcastPlayerFiredWorker(WorkerItem worker) => PlayerFiredWorker?.Invoke(worker);
    public static Action? NewDay;
    public static void BroadcastNewDay() => NewDay?.Invoke();
    public static Action? RefreshShops;
    public static void BroadcastRefreshShops() => RefreshShops?.Invoke();
    public static Action<Slots>? PlayerRepairingSlots;
    public static void BroadcastPlayerRepairingSlots(Slots slots) => PlayerRepairingSlots?.Invoke(slots);
}
