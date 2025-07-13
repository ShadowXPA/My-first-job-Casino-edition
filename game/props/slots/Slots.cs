using Godot;
using System;

public partial class Slots : StaticBody2D
{
    private PlayerInteractable? _playerInteractable;

    public override void _Ready()
    {
        ZIndex = (int)GlobalPosition.Y;
        _playerInteractable = GetNode<PlayerInteractable>("%PlayerInteractable");

        var repairButton = Utils.CreateActionButton("Repair", OnRepairPressed);
        var removeButton = Utils.CreateActionButton("Remove", OnRemovePressed);

        _playerInteractable.Actions.Add(repairButton);
        _playerInteractable.Actions.Add(removeButton);
    }

    private void OnRepairPressed()
    {
        GD.Print("Pressed REPAIR!");
    }

    private void OnRemovePressed()
    {
        GD.Print("Pressed REMOVE!");
        QueueFree();
    }
}
