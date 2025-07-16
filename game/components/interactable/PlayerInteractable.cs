using Godot;
using ProjectGJ.Scripts;
using System;
using System.Collections.Generic;

namespace ProjectGJ.Components.Interactable;

[GlobalClass]
public partial class PlayerInteractable : Area2D
{
    public readonly List<Button> Actions = [];
    public Action? EnterAction;
    public Action? ExitAction;

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
        EnterAction?.Invoke();
        SignalBus.BroadcastPlayerInteracting(Actions);
    }

    private void OnPlayerExited(Node2D body)
    {
        ExitAction?.Invoke();
		SignalBus.BroadcastPlayerStoppedInteracting(Actions);
    }
}
