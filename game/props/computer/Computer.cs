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
		var viewStatsButton = Utils.CreateActionButton("View stats", OnViewStatsPressed);

		_playerInteractable.Actions.Add(shopButton);
		_playerInteractable.Actions.Add(viewStatsButton);

		_playerInteractable.EnterAction += () => _animatedSprite.Play("on");
		_playerInteractable.ExitAction += () =>
		{
			_animatedSprite.Play("off");
			SignalBus.BroadcastShopButtonPressed(false);
		};
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
