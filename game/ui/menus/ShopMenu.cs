using Godot;
using Godot.NativeInterop;
using ProjectGJ.Characters.Worker;
using ProjectGJ.Scripts;
using ProjectGJ.Scripts.Items;
using ProjectGJ.Ui.Shop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectGJ.Ui.Menus;

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
		_buyList = GetNode<HBoxContainer>("%BuyList");
		_hireList = GetNode<HBoxContainer>("%HireList");
		_statueList = GetNode<HBoxContainer>("%StatueList");
		_exitButton = GetNode<Button>("%Exit");
		_exitButton.Pressed += () => OnShopPressed(false);
		AddCasinoGames(GameItems.CasinoGames);
		AddPeople([
			Utils.GenerateRandomWorker(),
			Utils.GenerateRandomWorker(),
			Utils.GenerateRandomWorker(),
			Utils.GenerateRandomWorker(),
			Utils.GenerateRandomWorker(),
		]);
		AddStatues(GameItems.Statues);

		SignalBus.ShopButtonPressed += OnShopPressed;
		SignalBus.PlayerHiredWorker += OnWorkerHired;
		SignalBus.PlayerBoughtStatue += OnStatueBought;
	}

	public override void _ExitTree()
	{
		SignalBus.ShopButtonPressed -= OnShopPressed;
		SignalBus.PlayerHiredWorker -= OnWorkerHired;
		SignalBus.PlayerBoughtStatue -= OnStatueBought;
	}

	public void AddCasinoGames(List<CasinoGameItem> items)
	{
		if (ShopItem is null) return;

		foreach (var item in items)
		{
			var shopItem = ShopItem.Instantiate<ShopItem>();
			_buyList?.AddChild(shopItem);
			var button = Utils.CreateActionButton("Buy", () => BuyItem(item), HorizontalAlignment.Center);
			shopItem.SetItem(item, button);
		}
	}

	public void AddPeople(List<WorkerItem> items)
	{
		if (ShopItem is null) return;

		foreach (var item in items)
		{
			var shopItem = ShopItem.Instantiate<ShopItem>();
			shopItem.SetMeta("id", GenerateShopItemId(item));
			_hireList?.AddChild(shopItem);
			var button = Utils.CreateActionButton("Hire", () => HirePerson(item), HorizontalAlignment.Center);
			shopItem.SetItem(item, button);
		}
	}

	public void AddStatues(List<StatueItem> items)
	{
		if (ShopItem is null) return;

		foreach (var item in items)
		{
			var shopItem = ShopItem.Instantiate<ShopItem>();
			shopItem.SetMeta("id", GenerateShopItemId(item));
			_statueList?.AddChild(shopItem);
			var button = Utils.CreateActionButton("Buy", () => BuyStatue(item), HorizontalAlignment.Center);
			shopItem.SetItem(item, button);
		}
	}

	public void ApplyPriceMultiplier(float priceMultiplier)
	{
		if (_buyList is not null)
			foreach (var item in _buyList.GetChildren().Cast<ShopItem>())
				item.SetPriceMultiplier(priceMultiplier);

		if (_hireList is not null)
			foreach (var item in _hireList.GetChildren().Cast<ShopItem>())
				item.SetPriceMultiplier(priceMultiplier);

		if (_statueList is not null)
			foreach (var item in _statueList.GetChildren().Cast<ShopItem>())
				item.SetPriceMultiplier(priceMultiplier);
	}

	private void BuyItem(CasinoGameItem item)
	{
		SignalBus.BroadcastPlayerBuyingCasinoGame(item);
	}

	private void HirePerson(WorkerItem item)
	{
		// TODO: check if can buy
		SignalBus.BroadcastPlayerHiringWorker(item);
		// _hireList?.RemoveChild(shopItem);
	}

	private void BuyStatue(StatueItem item)
	{
		// TODO: check if can buy
		SignalBus.BroadcastPlayerBuyingStatue(item);

		// if (shopItem.Actions is not null)
		// {
		// 	foreach (var action in shopItem.Actions.GetChildren())
		// 	{
		// 		shopItem.Actions.RemoveChild(action);
		// 	}
		// 	shopItem.Actions.AddChild(Utils.CreateActionButton("Bought", action: null, HorizontalAlignment.Center, disabled: true));
		// }

		// GD.PrintS("Bought statue:", item.Name, item.Price);
	}

	private void OnShopPressed(bool open)
	{
		Visible = open;
	}

	private void OnWorkerHired(WorkerItem worker)
	{
		if (_hireList is null) return;

		var shopItemId = GenerateShopItemId(worker);

		foreach (var shopItem in _hireList.GetChildren().Cast<ShopItem>())
		{
			var id = shopItem.GetMeta("id").AsString();
			if (id == shopItemId)
			{
				_hireList.RemoveChild(shopItem);
				break;
			}
		}
	}

	private void OnStatueBought(StatueItem statue)
	{
		if (_statueList is null) return;

		var shopItemId = GenerateShopItemId(statue);

		foreach (var shopItem in _statueList.GetChildren().Cast<ShopItem>())
		{
			var id = shopItem.GetMeta("id").AsString();
			if (id == shopItemId)
			{
				if (shopItem.Actions is not null)
				{
					foreach (var action in shopItem.Actions.GetChildren())
					{
						shopItem.Actions.RemoveChild(action);
					}
					shopItem.Actions.AddChild(Utils.CreateActionButton("Bought", action: null, HorizontalAlignment.Center, disabled: true));
				}

				break;
			}
		}

        if (statue.ShopPricesMultiplier is not null)
            ApplyPriceMultiplier((float)statue.ShopPricesMultiplier);
	}

	private string GenerateShopItemId(Item item)
	{
		return $"{item.Name}_{item.Description}_{item.Price}";
	}
}
