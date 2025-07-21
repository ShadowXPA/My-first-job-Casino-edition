using Godot;
using ProjectGJ.Components.Interactable;
using ProjectGJ.Scripts;

namespace ProjectGJ.Props.Slots;

public partial class Slots : StaticBody2D
{
    public Characters.Customer.Customer? Customer { get; private set; }
    public bool CanOccupy => Customer is null && _on;

    private AnimatedSprite2D? _animationSprite;
    private PlayerInteractable? _playerInteractable;
    private AudioStreamPlayer2D? _audioStreamPlayer;
    private Button? _repairButton;
    private Timer _timer = new();
    private bool _on;
    private bool _gambling;
    private bool _canGamble => _on && Customer is not null && !_gambling;

    public override void _Ready()
    {
        _animationSprite = GetNode<AnimatedSprite2D>("%Sprite");
        _playerInteractable = GetNode<PlayerInteractable>("%PlayerInteractable");
        _audioStreamPlayer = GetNode<AudioStreamPlayer2D>("%AudioStreamPlayer2D");

        _repairButton = Utils.CreateActionButton($"Repair (-${Constants.SLOT_MACHINE_REPAIR_FEE})", OnRepairPressed);
        _playerInteractable.Actions.Add(_repairButton);

        PowerOn();
        AddChild(_timer);
        _animationSprite.AnimationFinished += OnAnimationFinished;
        _timer.OneShot = true;
        _timer.Timeout += OnTimerTimeout;
    }

    public override void _ExitTree()
    {
        if (_animationSprite is not null)
            _animationSprite.AnimationFinished -= OnAnimationFinished;
        _timer.Timeout -= OnTimerTimeout;
    }

    public void Occupy(Characters.Customer.Customer? customer)
    {
        if (customer is not null && !_on) return;

        Customer = customer;

        if (_on && customer is null)
        {
            PlayAnimation("on");
            _audioStreamPlayer?.Stop();
        }
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
        if (!_canGamble) return;

        _gambling = true;
        PlayAnimation("pulling");
        _audioStreamPlayer?.Play();
    }

    public void Win()
    {
        PlayAnimation("win");
        Cooldown();
    }

    public void Lose()
    {
        PlayAnimation("lose");
        Cooldown();
    }

    public void PlayAnimation(string animation)
    {
        _animationSprite?.Play(animation);
    }

    private void Cooldown()
    {
        _audioStreamPlayer?.Stop();
        _timer.Start(2);
    }

    private void PowerOn()
    {
        if (_repairButton is null || _animationSprite is null) return;

        _repairButton.Visible = false;
        PlayAnimation("on");
        _on = true;
    }

    private void PowerOff()
    {
        if (_repairButton is null || _animationSprite is null) return;

        _repairButton.Visible = true;
        PlayAnimation("off");
        _audioStreamPlayer?.Stop();
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
            _timer.Start(GD.RandRange(Constants.MIN_GAMBLE_TIME, Constants.MAX_GAMBLE_TIME));
            PlayAnimation("rolling");
        }
    }

    private void OnTimerTimeout()
    {
        if (_animationSprite is null || Customer is null) return;

        if (_animationSprite.Animation == "rolling")
        {
            SignalBus.BroadcastCustomerGamblingSlots(this, Customer);
            return;
        }

        if (_animationSprite.Animation == "win" || _animationSprite.Animation == "lose")
        {
            PlayAnimation("on");
            _gambling = false;
            Gamble();
            return;
        }
    }
}
