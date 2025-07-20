using System.Collections.Generic;
using ProjectGJ.Scripts;

namespace ProjectGJ.Props.Blackjack;

public partial class Blackjack : WorkerStation
{
    private List<Characters.Customer.Customer> _customersPlaying = [];
    private bool _playing;

    public override void _Ready()
    {
        base._Ready();
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
        if (Worker is null || _customersPlaying.Count == 0) return;

        if (time % 10 == 0)
        {
            foreach (var customer in _customersPlaying)
                SignalBus.BroadcastCustomerGamblingRoulette(customer);
        }
    }
}
