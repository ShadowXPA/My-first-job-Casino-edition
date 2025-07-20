using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectGJ.Characters.Customer;
using ProjectGJ.Characters.Worker;

namespace ProjectGJ.Scripts;

public partial class WorkerStation : StaticBody2D
{
	public Node2D? WorkerSpawner { get; private set; }
    public Worker? Worker { get; private set; }
    public Node2D? CustomerSeats { get; private set; }
    public Dictionary<Customer, Node2D> Customers { get; private set; } = [];
    public bool HasEmptySeats => CustomerSeats?.GetChildren().Count != Customers.Count;

    public override void _Ready()
    {
        WorkerSpawner = GetNode<Node2D>("%WorkerSpawner");
        CustomerSeats = GetNode<Node2D>("%CustomerSeats");
    }

    public void SetWorker(Worker? worker)
    {
        Worker = worker;

        if (worker is null)
            foreach (var customerKV in Customers)
            {
                var customer = customerKV.Key;
                LeaveTable(customer);
                customer.NextActivity();
            }
    }

    public virtual Node2D? TryOccupyTable(Customer customer)
    {
        if (CustomerSeats is null || Customers.ContainsKey(customer) || !HasEmptySeats || Worker is null) return null;

        var emptySeats = CustomerSeats.GetChildren()
                            .Cast<Node2D>()
                            .Where(node => node.GetChildCount() == 0)
                            .ToList();
        var numberOfEmptySeats = emptySeats.Count;
        var seat = emptySeats[GD.RandRange(0, numberOfEmptySeats - 1)];

        Customers.Add(customer, seat);
        return seat;
    }

    public virtual void LeaveTable(Customer customer)
    {
        if (CustomerSeats is null || !Customers.TryGetValue(customer, out Node2D? seat)) return;

        Customers.Remove(customer);
    }
}
