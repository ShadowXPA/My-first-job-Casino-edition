using Godot;
using ProjectGJ.Scripts;
using ProjectGJ.Ui.Transactions;

namespace ProjectGJ.Ui.Menus;

public partial class TransactionsMenu : PanelContainer
{
	[Export]
	public PackedScene? TransactionItem;

	private ScrollContainer? _scrollContainer;
	private VBoxContainer? _transactionsList;
	private Button? _exitButton;

	public override void _Ready()
	{
		_scrollContainer = GetNode<ScrollContainer>("%ScrollContainer");
		_transactionsList = GetNode<VBoxContainer>("%Transactions");
		_exitButton = GetNode<Button>("%Exit");
		_exitButton.Pressed += () => OnTransactionsButtonPressed(false);

		SignalBus.TransactionsMenuButtonPressed += OnTransactionsButtonPressed;
		SignalBus.TransactionComplete += OnTransactionComplete;
	}

	public override void _ExitTree()
	{
		SignalBus.TransactionsMenuButtonPressed -= OnTransactionsButtonPressed;
		SignalBus.TransactionComplete -= OnTransactionComplete;
	}

	private void OnTransactionsButtonPressed(bool open)
	{
		Visible = open;
	}

	private void OnTransactionComplete(Transaction transaction)
	{
		if (TransactionItem is null || _transactionsList is null || _scrollContainer is null) return;

		var transactionItem = TransactionItem.Instantiate<TransactionItem>();
		_transactionsList.AddChild(transactionItem);
		transactionItem.SetTransaction(transaction);
	}
}
