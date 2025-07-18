using Godot;
using ProjectGJ.Characters.Customer;
using ProjectGJ.Components.Interactable;
using ProjectGJ.Scripts;
using System;

namespace ProjectGJ.Props.Slots;

public partial class Slots : StaticBody2D
{
    public Customer? Customer { get; private set; }

    private AnimatedSprite2D? _animationSprite;
    private PlayerInteractable? _playerInteractable;
    private Button? _repairButton;

    public override void _Ready()
    {
        _animationSprite = GetNode<AnimatedSprite2D>("%Sprite");
        _playerInteractable = GetNode<PlayerInteractable>("%PlayerInteractable");

        _repairButton = Utils.CreateActionButton("Repair", OnRepairPressed);
        _playerInteractable.Actions.Add(_repairButton);

        PowerOn();

        SignalBus.GameTimeChanged += OnTimeChanged;
    }

    public override void _ExitTree()
    {
        SignalBus.GameTimeChanged += OnTimeChanged;
    }

    public void Occupy(Customer customer)
    {
        Customer = customer;
    }

    private void PowerOn()
    {
        if (_repairButton is null || _animationSprite is null) return;

        _repairButton.Visible = false;
        _animationSprite.Play("on");
    }

    private void PowerOff()
    {
        if (_repairButton is null || _animationSprite is null) return;

        _repairButton.Visible = true;
        _animationSprite.Play("off");
    }

    private void OnRepairPressed()
    {
        GD.Print("Pressed REPAIR!");
        PowerOn();
    }

    private void OnTimeChanged(int elapsedTime)
    {
        if (GD.Randf() < 0.001f)
        {
        // TODO: kick the customer if it's here
            PowerOff();
            // SignalBus.BroadcastNotifyPlayer("Looks like a slot machine has broken down. Quick, fix it!");
        }
    }
}
