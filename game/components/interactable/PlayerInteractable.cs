using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class PlayerInteractable : Area2D
{
    public readonly List<Button> Actions = [];

    public override void _Ready()
    {
        BodyEntered += OnPlayerEntered;
        BodyExited += OnPlayerExited;
    }

    public override void _ExitTree()
    {
		SignalBus.BroadcastPlayerStoppedInteracting(Actions);
        BodyEntered -= OnPlayerEntered;
        BodyExited -= OnPlayerExited;
    }

    private void OnPlayerEntered(Node2D body)
    {
        SignalBus.BroadcastPlayerInteracting(Actions);
    }

    private void OnPlayerExited(Node2D body)
    {
		SignalBus.BroadcastPlayerStoppedInteracting(Actions);
    }
}
