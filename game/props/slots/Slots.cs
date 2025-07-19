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
    private Timer _timer = new();
    private bool _on;
    private bool _gambling;

    public override void _Ready()
    {
        _animationSprite = GetNode<AnimatedSprite2D>("%Sprite");
        _playerInteractable = GetNode<PlayerInteractable>("%PlayerInteractable");

        _repairButton = Utils.CreateActionButton($"Repair (-${Constants.SLOT_MACHINE_REPAIR_FEE})", OnRepairPressed);
        _playerInteractable.Actions.Add(_repairButton);

        PowerOn();
        AddChild(_timer);
        _animationSprite.AnimationFinished += OnAnimationFinished;
        _timer.Timeout += OnPlayerGambled;
    }

    public override void _ExitTree()
    {
        if (_animationSprite is not null)
            _animationSprite.AnimationFinished -= OnAnimationFinished;
        _timer.Timeout -= OnPlayerGambled;
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

    public void Gamble()
    {
        if (!_on || Customer is null || _gambling) return;

        _gambling = true;
        _animationSprite?.Play("pulling");
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

    private void OnAnimationFinished()
    {
        if (_animationSprite is null) return;

        if (_animationSprite.Animation == "pulling")
        {
            _timer.WaitTime = GD.RandRange(Constants.MIN_GAMBLE_TIME, Constants.MAX_GAMBLE_TIME);
            _timer.Start();
            _animationSprite.Play("rolling");
        }
    }

    private void OnPlayerGambled()
    {
        // TODO: signal that customer gambled, GameManager decides if customer won or lost (also apply bonuses), then play animation, 1 sec later is available for gamble
        _animationSprite?.Play("win");
        // SignalBus.BroadcastCustomerGambled(Customer);
    }
}
