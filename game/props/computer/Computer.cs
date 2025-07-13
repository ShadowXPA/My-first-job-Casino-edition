using Godot;
using System;

public partial class Computer : StaticBody2D
{
	private PlayerInteractable? _playerInteractable;

	public override void _Ready()
	{
        ZIndex = (int)GlobalPosition.Y;
		_playerInteractable = GetNode<PlayerInteractable>("%PlayerInteractable");

		var shopButton = Utils.CreateActionButton("Shop", OnShopPressed);
		var viewStatsButton = Utils.CreateActionButton("View stats", OnViewStatsPressed);

		_playerInteractable.Actions.Add(shopButton);
		_playerInteractable.Actions.Add(viewStatsButton);
	}

	private void OnShopPressed()
	{
		GD.Print("Pressed SHOP!");
		SignalBus.BroadcastShopButtonPressed();
	}

	private void OnViewStatsPressed()
	{
		GD.Print("Pressed STATS!");
	}
}
