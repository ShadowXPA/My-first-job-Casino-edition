using Godot;
using ProjectGJ.Scripts;
using System;

namespace ProjectGJ.Ui.Transactions;

public partial class TransactionItem : MarginContainer
{
	private Label? _description;
	private Label? _day;
	private Label? _beforeTransaction;
	private Label? _afterTransaction;
	private Label? _transactionAmount;

	public override void _Ready()
	{
		_description = GetNode<Label>("%Description");
		_day = GetNode<Label>("%Day");
		_beforeTransaction = GetNode<Label>("%BeforeTransaction");
		_afterTransaction = GetNode<Label>("%AfterTransaction");
		_transactionAmount = GetNode<Label>("%TransactionAmount");
	}

	public void SetTransaction(Transaction transaction)
	{
		var redColor = new Color(1, 0, 0, 1);
		var whiteColor = new Color(1, 1, 1, 1);
		var greenColor = new Color(0, 1, 0, 1);

		if (_description is not null)
		{
			_description.Text = transaction.Description;
		}

		if (_day is not null)
		{
			_day.Text = $"{transaction.Day}";
		}

		if (_beforeTransaction is not null)
		{
			_beforeTransaction.Text = $"{(transaction.AmountBeforeTransaction < 0 ? "-" : "")}${Mathf.Abs(transaction.AmountBeforeTransaction)}";
			_beforeTransaction.Modulate = transaction.AmountBeforeTransaction < 0 ? redColor : whiteColor;
		}

		if (_afterTransaction is not null)
		{
			_afterTransaction.Text = $"{(transaction.AmountAfterTransaction < 0 ? "-" : "")}${Mathf.Abs(transaction.AmountAfterTransaction)}";
			_afterTransaction.Modulate = transaction.AmountAfterTransaction < 0 ? redColor : whiteColor;
		}

		if (_transactionAmount is not null)
		{
			_transactionAmount.Text = $"{(transaction.TransactionAmount < 0 ? "-" : "+")}${Mathf.Abs(transaction.TransactionAmount)}";
			_transactionAmount.Modulate = transaction.TransactionAmount < 0 ? redColor : greenColor;
		}
	}
}
