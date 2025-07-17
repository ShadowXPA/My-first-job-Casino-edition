using Godot;
using ProjectGJ.Components.Interactable;
using ProjectGJ.Scripts;
using System;

namespace ProjectGJ.Props.Computer;

public partial class Computer : StaticBody2D
{
	private AnimatedSprite2D? _animatedSprite;
	private PlayerInteractable? _playerInteractable;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("%Sprite");
		_playerInteractable = GetNode<PlayerInteractable>("%PlayerInteractable");

		var shopButton = Utils.CreateActionButton("Shop", OnShopPressed);
		var inventoryButton = Utils.CreateActionButton("Inventory", OnInventoryPressed);
		var viewTransactionsButton = Utils.CreateActionButton("View transactions", OnViewTransactionsPressed);

		_playerInteractable.Actions.Add(shopButton);
		_playerInteractable.Actions.Add(inventoryButton);
		_playerInteractable.Actions.Add(viewTransactionsButton);

		_playerInteractable.EnterAction += () => _animatedSprite.Play("on");
		_playerInteractable.ExitAction += () =>
		{
			_animatedSprite.Play("off");
			SignalBus.BroadcastShopMenuButtonPressed(false);
			SignalBus.BroadcastInventoryMenuButtonPressed(false);
			SignalBus.BroadcastTransactionsMenuButtonPressed(false);
		};
	}

	private void OnShopPressed()
	{
		SignalBus.BroadcastShopMenuButtonPressed();
	}

	private void OnInventoryPressed()
	{
		SignalBus.BroadcastInventoryMenuButtonPressed();
	}

	private void OnViewTransactionsPressed()
	{
		SignalBus.BroadcastTransactionsMenuButtonPressed();
	}
}
