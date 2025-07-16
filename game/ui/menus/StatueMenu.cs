using Godot;
using ProjectGJ.Scripts;
using ProjectGJ.Scripts.Items;
using ProjectGJ.Ui.Shop;
using System;
using System.Collections.Generic;

namespace ProjectGJ.Ui.Menus;

public partial class StatueMenu : VBoxContainer
{
	[Export]
	public PackedScene? ShopItem;

	private HBoxContainer? _statueList;
	private Button? _exitButton;

	public override void _Ready()
	{
		_statueList = GetNode<HBoxContainer>("%StatueList");
		_exitButton = GetNode<Button>("%Exit");
		_exitButton.Pressed += () => Visible = false;
		// AddStatues(GameItems.Statues); // TODO: change this to acquired statues
		SignalBus.StatueMenuButtonPressed += OnStatueInventoryPressed;
		SignalBus.PlayerBoughtStatue += OnPlayerBoughtStatue;
	}

	public override void _ExitTree()
	{
		SignalBus.StatueMenuButtonPressed -= OnStatueInventoryPressed;
		SignalBus.PlayerBoughtStatue -= OnPlayerBoughtStatue;
	}

	public void AddStatues(List<StatueItem> items)
	{
		foreach (var item in items)
		{
			var shopItem = ShopItem?.Instantiate<ShopItem>();
			_statueList?.AddChild(shopItem);
			var button = Utils.CreateActionButton("Select", () => SelectedStatue(item), HorizontalAlignment.Center);
			shopItem?.SetItem(item, button);
		}
	}

	private void SelectedStatue(StatueItem statueItem)
	{
		SignalBus.BroadcastPlayerSelectedStatue(statueItem);
	}

	private void OnStatueInventoryPressed(bool open)
	{
		Visible = open;
	}

	private void OnPlayerBoughtStatue(StatueItem statue)
	{
		AddStatues([statue]);
	}
}
