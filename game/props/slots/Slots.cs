using Godot;
using ProjectGJ.Characters.Customer;
using ProjectGJ.Components.Interactable;
using ProjectGJ.Scripts;
using System;

namespace ProjectGJ.Props.Slots;

public partial class Slots : StaticBody2D
{
    public Characters.Customer.Customer? Customer { get; private set; }
    public bool CanOccupy => Customer is null && _on;

    private AnimatedSprite2D? _animationSprite;
    private PlayerInteractable? _playerInteractable;
    private Button? _repairButton;
    private bool _on;

    public override void _Ready()
    {
        _animationSprite = GetNode<AnimatedSprite2D>("%Sprite");
        _playerInteractable = GetNode<PlayerInteractable>("%PlayerInteractable");

        _repairButton = Utils.CreateActionButton($"Repair (-${Constants.SLOT_MACHINE_REPAIR_FEE})", OnRepairPressed);
        _playerInteractable.Actions.Add(_repairButton);

        PowerOn();
    }

    public void Occupy(Characters.Customer.Customer? customer)
    {
        if (!_on) return;

        Customer = customer;
    }

    public void Break()
    {
        PowerOff();
        Customer?.NextActivity();
        Occupy(null);
    }

    public void Repair()
    {
        PowerOn();
    }

    private void PowerOn()
    {
        if (_repairButton is null || _animationSprite is null) return;

        _repairButton.Visible = false;
        _animationSprite.Play("on");
        _on = true;
    }

    private void PowerOff()
    {
        if (_repairButton is null || _animationSprite is null) return;

        _repairButton.Visible = true;
        _animationSprite.Play("off");
        _on = false;
    }

    private void OnRepairPressed()
    {
        SignalBus.BroadcastPlayerRepairingSlots(this);
    }
}
