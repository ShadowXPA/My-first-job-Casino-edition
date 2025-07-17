using Godot;
using ProjectGJ.Scripts;
using ProjectGJ.Ui.Transactions;
using System;

namespace ProjectGJ.Ui.Menus;

public partial class TransactionsMenu : PanelContainer
{
	[Export]
	public PackedScene? TransactionItem;

	private VBoxContainer? _transactionsList;
	private Button? _exitButton;

	public override void _Ready()
	{
		_transactionsList = GetNode<VBoxContainer>("%Transactions");
		_exitButton = GetNode<Button>("%Exit");
		_exitButton.Pressed += () => OnTransactionsButtonPressed(false);

		SignalBus.TransactionsMenuButtonPressed += OnTransactionsButtonPressed;
		SignalBus.PlayerMoneyTransaction += OnTransaction;
	}

	public override void _ExitTree()
	{
		SignalBus.TransactionsMenuButtonPressed -= OnTransactionsButtonPressed;
		SignalBus.PlayerMoneyTransaction -= OnTransaction;
	}

	private void OnTransactionsButtonPressed(bool open)
	{
		Visible = open;
	}

	private void OnTransaction(Transaction transaction)
	{
		GD.PrintS(transaction);

		if (TransactionItem is null || _transactionsList is null) return;

		var transactionItem = TransactionItem.Instantiate<TransactionItem>();
		_transactionsList.AddChild(transactionItem);
		transactionItem.SetTransaction(transaction);
	}
}
