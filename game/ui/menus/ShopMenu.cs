using Godot;
using System;
using System.Collections.Generic;

public partial class ShopMenu : VBoxContainer
{
	[Export]
	public PackedScene? ShopItem;

	private HBoxContainer? _buyList;
	private HBoxContainer? _hireList;
	private HBoxContainer? _statueList;
	private Button? _exitButton;

	public override void _Ready()
	{
		SignalBus.ShopButtonPressed += OnShopPressed;
		_buyList = GetNode<HBoxContainer>("%BuyList");
		_hireList = GetNode<HBoxContainer>("%HireList");
		_statueList = GetNode<HBoxContainer>("%StatueList");
		_exitButton = GetNode<Button>("%Exit");
		_exitButton.Pressed += () => OnShopPressed(false);
		AddItems(GameItems.ShopItems);
		AddStatues(GameItems.ShopStatueItems);
	}

	public override void _ExitTree()
	{
		SignalBus.ShopButtonPressed -= OnShopPressed;
	}

	public void AddItems(List<Item> items)
	{
		foreach (var item in items)
		{
			var shopItem = ShopItem?.Instantiate<ShopItem>();
			_buyList?.AddChild(shopItem);
			var button = Utils.CreateActionButton("Buy", () => BuyItem(item), HorizontalAlignment.Center);
			shopItem?.SetItem(item.Name, $"${item.Price}", item.Resource, button);
		}
	}

	public void AddPeople(List<Item> items)
	{
		foreach (var item in items)
		{
			var shopItem = ShopItem?.Instantiate<ShopItem>();
			_hireList?.AddChild(shopItem);
			var button = Utils.CreateActionButton("Hire", () => BuyItem(item), HorizontalAlignment.Center);
			shopItem?.SetItem(item.Name, $"${item.Price}/30 days", item.Resource, button);
		}
	}

	public void AddStatues(List<Item> items)
	{
		foreach (var item in items)
		{
			var shopItem = ShopItem?.Instantiate<ShopItem>();
			_statueList?.AddChild(shopItem);
			var button = Utils.CreateActionButton("Buy", () => BuyStatue(item), HorizontalAlignment.Center);
			shopItem?.SetItem(item.Name, $"${item.Price}", item.Resource, button);
		}
	}

	private void BuyItem(Item item)
	{
		GD.PrintS("Bought:", item.Name, item.Price);
	}

	private void HirePerson(Item item)
	{
		GD.PrintS("Hired:", item.Name, item.Price);
	}

	private void BuyStatue(Item item)
	{
		GD.PrintS("Bought statue:", item.Name, item.Price);
	}

	private void OnShopPressed(bool open)
	{
		Visible = open;
	}
}
