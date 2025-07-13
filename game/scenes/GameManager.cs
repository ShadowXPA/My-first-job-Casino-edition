using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
    public override void _Ready()
    {
        SignalBus.PlayerInteracting += PlayerInteracting;
        SignalBus.PlayerStoppedInteracting += PlayerStoppedInteracting;
    }

    public override void _ExitTree()
    {
        SignalBus.PlayerInteracting -= PlayerInteracting;
        SignalBus.PlayerStoppedInteracting -= PlayerStoppedInteracting;
    }

    private void PlayerInteracting(List<Button> actions)
    {
        GD.PrintS("Player interacting.");
    }

    private void PlayerStoppedInteracting(List<Button> actions)
    {
        GD.PrintS("Player stopped interacting.");
    }
}
