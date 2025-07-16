using Godot;
using ProjectGJ.Scripts;
using ProjectGJ.Scripts.Items;
using ProjectGJ.Ui.Shop;
using System;

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
		_workersList = GetNode<GridContainer>("%WorkersList");
		_statuesList = GetNode<GridContainer>("%StatuesList");
		_exitButton = GetNode<Button>("%Exit");
		_exitButton.Pressed += () => OnInventoryButtonPressed(false);

		SignalBus.InventoryMenuButtonPressed += OnInventoryButtonPressed;
		SignalBus.PlayerBoughtCasinoGame += OnPlayerBoughtCasinoGame;
		SignalBus.PlayerHiredWorker += OnPlayerHiredWorker;
		SignalBus.PlayerBoughtStatue += OnPlayerBoughtStatue;
	}

	public override void _ExitTree()
	{
		SignalBus.InventoryMenuButtonPressed -= OnInventoryButtonPressed;
		SignalBus.PlayerBoughtCasinoGame -= OnPlayerBoughtCasinoGame;
		SignalBus.PlayerHiredWorker -= OnPlayerHiredWorker;
		SignalBus.PlayerBoughtStatue -= OnPlayerBoughtStatue;
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
		var button = Utils.CreateActionButton($"Sell (+${Mathf.FloorToInt(casinoGame.FinalPrice * Constants.SELL_PERCENTAGE)})", () => SellCasinoGame(casinoGame), HorizontalAlignment.Center);
		shopItem.SetItem(casinoGame, button);
	}

	private void SellCasinoGame(CasinoGameItem casinoGame)
	{
	}

	private void OnPlayerHiredWorker(WorkerItem worker)
	{
		if (ShopItem is null) return;

		var shopItem = ShopItem.Instantiate<ShopItem>();
		_workersList?.AddChild(shopItem);
		// TODO: this is not gonna work well, the button will not update based on the days that have passed, since it only "updates" when the player is hired
		var button = Utils.CreateActionButton($"Fire (-${Mathf.FloorToInt(worker.FinalPrice / (30 * worker.DaysWorked))})", () => FireWorker(worker), HorizontalAlignment.Center);
		shopItem.SetItem(worker, button);
	}

	private void FireWorker(WorkerItem worker)
	{
	}

	private void OnPlayerBoughtStatue(StatueItem statue)
	{
		if (ShopItem is null) return;

		var shopItem = ShopItem.Instantiate<ShopItem>();
		_statuesList?.AddChild(shopItem);
		shopItem.SetItem(statue);
	}
}
