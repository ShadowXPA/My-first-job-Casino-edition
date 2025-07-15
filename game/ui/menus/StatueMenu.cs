using Godot;
using System;
using System.Collections.Generic;

public partial class StatueMenu : VBoxContainer
{
	[Export]
	public PackedScene? ShopItem;

	private HBoxContainer? _statueList;
	private Button? _exitButton;

	public override void _Ready()
	{
		SignalBus.StatueInventoryButtonPressed += OnStatueInventoryPressed;
		_statueList = GetNode<HBoxContainer>("%StatueList");
		_exitButton = GetNode<Button>("%Exit");
		_exitButton.Pressed += () => Visible = false;
		AddStatues(GameItems.Statues); // TODO: change this to acquired statues
	}

	public override void _ExitTree()
	{
		SignalBus.StatueInventoryButtonPressed -= OnStatueInventoryPressed;
	}

	public void AddStatues(List<StatueItem> items)
	{
		foreach (var item in items)
		{
			var shopItem = ShopItem?.Instantiate<ShopItem>();
			_statueList?.AddChild(shopItem);
			var button = Utils.CreateActionButton("Select", () => SelectedStatue(item), HorizontalAlignment.Center);
			shopItem?.SetItem(item.Name, item.Description, item.Resource, button);
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
}
