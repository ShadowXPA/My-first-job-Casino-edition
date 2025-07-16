using Godot;
using ProjectGJ.Components.Interactable;
using ProjectGJ.Scripts;
using System;

namespace ProjectGJ.Props.Slots;

public partial class Slots : StaticBody2D
{
    private AnimatedSprite2D? _animationSprite;
    private PlayerInteractable? _playerInteractable;
    private Button? _togglePowerButton;
    private bool _on;

    public override void _Ready()
    {
        _animationSprite = GetNode<AnimatedSprite2D>("%Sprite");
        _playerInteractable = GetNode<PlayerInteractable>("%PlayerInteractable");

        _togglePowerButton = Utils.CreateActionButton("Turn on", OnTogglePower);
        var repairButton = Utils.CreateActionButton("Repair", OnRepairPressed);
        var sellButton = Utils.CreateActionButton("Sell", OnSell);

        _playerInteractable.Actions.Add(_togglePowerButton);
        _playerInteractable.Actions.Add(repairButton);
        _playerInteractable.Actions.Add(sellButton);
    }

    private void OnTogglePower()
    {
        if (_togglePowerButton is null) return;

        _on = !_on;
        _togglePowerButton.Text = $"Turn {(_on ? "off" : "on")}";
        _animationSprite?.Play(_on ? "on" : "off");
    }

    private void OnRepairPressed()
    {
        GD.Print("Pressed REPAIR!");
    }

    private void OnSell()
    {
        GD.Print("Pressed SELL!");
        QueueFree();
    }
}
