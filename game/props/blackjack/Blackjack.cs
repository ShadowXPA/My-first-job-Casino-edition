using System.Collections.Generic;
using Godot;
using ProjectGJ.Scripts;

namespace ProjectGJ.Props.Blackjack;

public partial class Blackjack : WorkerStation
{
    private List<Characters.Customer.Customer> _customersPlaying = [];
    private AudioStreamPlayer2D? _audioStreamPlayer;
    private bool _playing;

    public override void _Ready()
    {
        base._Ready();
        _audioStreamPlayer = GetNode<AudioStreamPlayer2D>("%AudioStreamPlayer2D");
        SignalBus.GameTimeChanged += OnTimeChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        SignalBus.GameTimeChanged -= OnTimeChanged;
    }

    public void Play(Characters.Customer.Customer customer)
    {
        if (Worker is null) return;

        _customersPlaying.Add(customer);
    }

    public override void LeaveTable(Characters.Customer.Customer customer)
    {
        base.LeaveTable(customer);
        _customersPlaying.Remove(customer);
    }

    private void OnTimeChanged(int time)
    {
        if (Worker is null || _customersPlaying.Count == 0)
        {
            if (_audioStreamPlayer is not null && _audioStreamPlayer.Playing)
                _audioStreamPlayer.Stop();
            return;
        }

        if (time % 30 == 0)
        {
            if (_audioStreamPlayer is not null && !_audioStreamPlayer.Playing)
                _audioStreamPlayer.Play();
            foreach (var customer in _customersPlaying)
                SignalBus.BroadcastCustomerGamblingBlackjack(customer);
        }
    }
}
