using System.Collections.Generic;
using Godot;
using ProjectGJ.Scripts;

namespace ProjectGJ.Props.Bar;

public partial class Bar : WorkerStation
{
    private Dictionary<Characters.Customer.Customer, int> _customersDrinking = [];

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

    public void Drink(Characters.Customer.Customer customer)
    {
        _customersDrinking.Add(customer, GD.RandRange(5, 10));
    }

    public override void LeaveTable(Characters.Customer.Customer customer)
    {
        base.LeaveTable(customer);
        _customersDrinking.Remove(customer);
    }

    private void OnTimeChanged(int time)
    {
        foreach (var customer in _customersDrinking)
        {
            if (time % customer.Value == 0)
            {
                SignalBus.BroadcastCustomerDrinking(customer.Key);
            }
        }
    }
}
