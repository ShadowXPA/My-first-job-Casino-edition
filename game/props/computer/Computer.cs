using Godot;
using System;

public partial class Computer : StaticBody2D
{
	private PlayerInteractable? _playerInteractable;

	public override void _Ready()
	{
        ZIndex = (int)GlobalPosition.Y;
		_playerInteractable = GetNode<PlayerInteractable>("%PlayerInteractable");

		var buyButton = Utils.CreateActionButton("Buy", OnBuyPressed);
		var hireButton = Utils.CreateActionButton("Hire", OnHirePressed);
		var viewStatsButton = Utils.CreateActionButton("View stats", OnViewStatsPressed);

		_playerInteractable.Actions.Add(buyButton);
		_playerInteractable.Actions.Add(hireButton);
		_playerInteractable.Actions.Add(viewStatsButton);
	}

	private void OnBuyPressed()
	{
		GD.Print("Pressed BUY!");
	}

	private void OnHirePressed()
	{
		GD.Print("Pressed HIRE!");
	}

	private void OnViewStatsPressed()
	{
		GD.Print("Pressed STATS!");
	}
}
