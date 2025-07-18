using Godot;
using ProjectGJ.Scripts;
using ProjectGJ.Scripts.Items;
using ProjectGJ.Ui.Shop;
using System;
using System.Linq;

namespace ProjectGJ.Ui.Menus;

public partial class InventoryMenu : PanelContainer
{
	[Export]
	public PackedScene? ShopItem;

	private GridContainer? _gamesList;
	private GridContainer? _workersList;
	private GridContainer? _statuesList;
	private Button? _exitButton;

	public override void _Ready()
	{
		_gamesList = GetNode<GridContainer>("%GamesList");
		var gamesListParent = _gamesList.GetParent().GetParent();
		gamesListParent.QueueFree();
		_gamesList = null;
		_workersList = GetNode<GridContainer>("%WorkersList");
		_statuesList = GetNode<GridContainer>("%StatuesList");
		_exitButton = GetNode<Button>("%Exit");
		_exitButton.Pressed += () => OnInventoryButtonPressed(false);

		SignalBus.InventoryMenuButtonPressed += OnInventoryButtonPressed;
		SignalBus.PlayerBoughtCasinoGame += OnPlayerBoughtCasinoGame;
		SignalBus.PlayerHiredWorker += OnPlayerHiredWorker;
		SignalBus.PlayerBoughtStatue += OnPlayerBoughtStatue;
		SignalBus.NewDay += OnNewDay;
	}

	public override void _ExitTree()
	{
		SignalBus.InventoryMenuButtonPressed -= OnInventoryButtonPressed;
		SignalBus.PlayerBoughtCasinoGame -= OnPlayerBoughtCasinoGame;
		SignalBus.PlayerHiredWorker -= OnPlayerHiredWorker;
		SignalBus.PlayerBoughtStatue -= OnPlayerBoughtStatue;
		SignalBus.NewDay -= OnNewDay;
	}

	private void OnInventoryButtonPressed(bool open)
	{
		Visible = open;
	}

	private void OnPlayerBoughtCasinoGame(CasinoGameItem casinoGame)
	{
		if (ShopItem is null) return;

		var shopItem = ShopItem.Instantiate<ShopItem>();
		_gamesList?.AddChild(shopItem);
		var button = Utils.CreateActionButton($"Sell (+${Mathf.FloorToInt(casinoGame.FinalPrice * Constants.SELL_PERCENTAGE)})", () => SellCasinoGame(casinoGame, shopItem), HorizontalAlignment.Center);
		shopItem.SetItem(casinoGame, button);
	}

	private void SellCasinoGame(CasinoGameItem casinoGame, ShopItem shopItem)
	{
		_gamesList?.RemoveChild(shopItem);
		shopItem.QueueFree();
		SignalBus.BroadcastPlayerSoldCasinoGame(casinoGame);
	}

	private void OnPlayerHiredWorker(WorkerItem worker)
	{
		if (ShopItem is null) return;

		var shopItem = ShopItem.Instantiate<ShopItem>();
		_workersList?.AddChild(shopItem);
		var button = Utils.CreateActionButton($"Fire (-${Mathf.FloorToInt(worker.FinalPrice / 30 * (worker.DaysWorked % 30))})", () => FireWorker(worker, shopItem), HorizontalAlignment.Center);
		shopItem.SetItem(worker, button);
	}

	private void FireWorker(WorkerItem worker, ShopItem shopItem)
	{
		_workersList?.RemoveChild(shopItem);
		shopItem.QueueFree();
		SignalBus.BroadcastPlayerFiredWorker(worker);
	}

	private void OnPlayerBoughtStatue(StatueItem statue)
	{
		if (ShopItem is null) return;

		var shopItem = ShopItem.Instantiate<ShopItem>();
		_statuesList?.AddChild(shopItem);
		shopItem.SetItem(statue);
	}

	private void OnNewDay()
	{
		if (_workersList is null) return;

		foreach (var workerItem in _workersList.GetChildren().Cast<ShopItem>())
		{
			if (workerItem.Actions is not null)
			{
				var worker = (WorkerItem)workerItem.GetItem()!;
				var button = workerItem.Actions.GetChild<Button>(0);
				button.Text = $"Fire (-${Mathf.FloorToInt(worker.FinalPrice / 30 * (worker.DaysWorked % 30))})";
			}
		}
	}
}
