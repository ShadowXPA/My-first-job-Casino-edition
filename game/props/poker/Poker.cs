using Godot;
using ProjectGJ.Characters.Customer;
using ProjectGJ.Characters.Worker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectGJ.Props.Poker;

public partial class Poker : StaticBody2D
{
	public Node2D? DealerSpawner { get; private set; }
	public Node? CustomerSeats { get; private set; }
	public Worker? Worker { get; private set; }
	public Dictionary<Customer, Node2D> Customers { get; private set; } = [];
	public bool HasEmptySeats => CustomerSeats?.GetChildren().Any(node => node.GetChildCount() == 0) ?? false;

	public override void _Ready()
	{
		DealerSpawner = GetNode<Node2D>("%DealerSpawner");
		CustomerSeats = GetNode<Node>("%CustomerSeats");
	}

	public void SetWorker(Worker worker)
	{
		Worker = worker;
	}

	public Node2D? TryOccupyTable(Customer customer)
	{
		if (CustomerSeats is null || Customers.ContainsKey(customer) || !HasEmptySeats) return null;

		var emptySeats = CustomerSeats.GetChildren()
							.Cast<Node2D>()
							.Where(node => node.GetChildCount() == 0)
							.ToList();
		var numberOfEmptySeats = emptySeats.Count;
		var seat = emptySeats[GD.RandRange(0, numberOfEmptySeats - 1)];

		seat.AddChild(customer);
		Customers.Add(customer, seat);
		return seat;
	}

	public void LeaveTable(Customer customer)
	{
		if (CustomerSeats is null || !Customers.TryGetValue(customer, out Node2D? seat)) return;

		Customers.Remove(customer);
		seat.RemoveChild(customer);
	}
}
