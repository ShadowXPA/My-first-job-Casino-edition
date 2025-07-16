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

		SignalBus.ShopMenuButtonPressed += OnShopPressed;
		SignalBus.PlayerHiredWorker += OnWorkerHired;
		SignalBus.PlayerBoughtStatue += OnStatueBought;
	}

	public override void _ExitTree()
	{
		SignalBus.ShopMenuButtonPressed -= OnShopPressed;
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
		SignalBus.BroadcastPlayerHiringWorker(item);
	}

	private void BuyStatue(StatueItem item)
	{
		SignalBus.BroadcastPlayerBuyingStatue(item);
	}

	private void OnShopPressed(bool open)
	{
		Visible = open;
	}

	private void OnWorkerHired(WorkerItem worker)
	{
		if (_hireList is null) return;

		foreach (var shopItem in _hireList.GetChildren().Cast<ShopItem>())
		{
			if (shopItem.IsItemEqual(worker))
			{
				_hireList.RemoveChild(shopItem);
				break;
			}
		}
	}

	private void OnStatueBought(StatueItem statue)
	{
		if (_statueList is null) return;

		foreach (var shopItem in _statueList.GetChildren().Cast<ShopItem>())
		{
			if (shopItem.IsItemEqual(statue))
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
}
