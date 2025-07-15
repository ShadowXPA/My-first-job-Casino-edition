using System;
using System.Collections.Generic;
using Godot;

public static class SignalBus
{
    public static Action<List<Button>>? PlayerInteracting;
    public static Action<List<Button>>? PlayerStoppedInteracting;
    public static Action<Transaction>? PlayerMoneyTransaction;
    // public static Action<Transaction>? CustomerMoneyTransaction;

    public static Action? ShopButtonPressed;

    public static void BroadcastPlayerInteracting(List<Button> actions) => PlayerInteracting?.Invoke(actions);
    public static void BroadcastPlayerStoppedInteracting(List<Button> actions) => PlayerStoppedInteracting?.Invoke(actions);
    public static void BroadcastPlayerMoneyTransaction(Transaction transaction) => PlayerMoneyTransaction?.Invoke(transaction);

    public static void BroadcastShopButtonPressed() => ShopButtonPressed?.Invoke();
}
