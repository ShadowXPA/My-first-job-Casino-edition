using System;
using System.Collections.Generic;
using Godot;

public static class SignalBus
{
    public static Action<List<Button>>? PlayerInteracting;
    public static Action<List<Button>>? PlayerStoppedInteracting;
    public static Action<Transaction>? PlayerMoneyTransaction;
    // public static Action<Transaction>? CustomerMoneyTransaction;
    public static Action<StatueItem>? PlayerSelectedStatue;

    public static Action<bool>? ShopButtonPressed;
    public static Action<bool>? StatueInventoryButtonPressed;

    public static void BroadcastPlayerInteracting(List<Button> actions) => PlayerInteracting?.Invoke(actions);
    public static void BroadcastPlayerStoppedInteracting(List<Button> actions) => PlayerStoppedInteracting?.Invoke(actions);
    public static void BroadcastPlayerMoneyTransaction(Transaction transaction) => PlayerMoneyTransaction?.Invoke(transaction);
    public static void BroadcastPlayerSelectedStatue(StatueItem item) => PlayerSelectedStatue?.Invoke(item);

    public static void BroadcastShopButtonPressed(bool open = true) => ShopButtonPressed?.Invoke(open);
    public static void BroadcastStatueInventoryButtonPressed(bool open = true) => StatueInventoryButtonPressed?.Invoke(open);
}
