using System;
using System.Collections.Generic;
using Godot;

public static class SignalBus
{
    public static Action<List<Button>>? PlayerInteracting;
    public static Action<List<Button>>? PlayerStoppedInteracting;

    public static void BroadcastPlayerInteracting(List<Button> actions) => PlayerInteracting?.Invoke(actions);
    public static void BroadcastPlayerStoppedInteracting(List<Button> actions) => PlayerStoppedInteracting?.Invoke(actions);
}
