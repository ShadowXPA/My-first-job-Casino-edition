using System.Collections.Generic;
using Godot;
using ProjectGJ.Scripts;

namespace ProjectGJ.Ui.Hud;

public partial class Hud : Control
{
	public Label? Time { get; private set; }
	public Label? Day { get; private set; }
	public Label? Money { get; private set; }
	public Control? Transactions { get; private set; }
	public Control? Notifications { get; private set; }
	public VBoxContainer? Actions { get; private set; }

	public override void _Ready()
	{
		Time = GetNode<Label>("%Time");
		Day = GetNode<Label>("%Day");
		Money = GetNode<Label>("%Money");
		Transactions = GetNode<Control>("%Transactions");
		Notifications = GetNode<Control>("%Notifications");
		Actions = GetNode<VBoxContainer>("%Actions");
		SignalBus.PlayerInteracting += OnPlayerInteracting;
		SignalBus.PlayerStoppedInteracting += OnPlayerStoppedInteracting;
		SignalBus.GameTimeChanged += OnGameTimeChanged;
		SignalBus.TransactionComplete += OnTransactionComplete;
		SignalBus.NotifyPlayer += OnNotifyPlayer;
	}

	public override void _ExitTree()
	{
		SignalBus.PlayerInteracting -= OnPlayerInteracting;
		SignalBus.PlayerStoppedInteracting -= OnPlayerStoppedInteracting;
		SignalBus.GameTimeChanged -= OnGameTimeChanged;
		SignalBus.TransactionComplete -= OnTransactionComplete;
		SignalBus.NotifyPlayer -= OnNotifyPlayer;
	}

	public void SetTimeAndDay(int time)
	{
		if (Time is null || Day is null) return;

		Time.Text = Utils.GetTimeFromElapsedTime(time);
		Day.Text = $"Day {Utils.GetDayFromElapsedTime(time)}";
	}

	public void SetMoney(int money)
	{
		if (Money is null) return;

		Money.Text = $"{(money < 0 ? "-" : "")}${Mathf.Abs(money)}";
		Money.Modulate = money < 0 ? new Color(1, 0, 0, 1) : new Color(1, 1, 1, 1);
	}

	public void AddTransaction(int amount)
	{
		if (Transactions is null) return;

		var label = new Label();
		label.Text = $"{(amount < 0 ? "-" : "+")}${Mathf.Abs(amount)}";
		label.Modulate = new Color(amount < 0 ? 1 : 0, amount > 0 ? 1 : 0, amount == 0 ? 1 : 0, 1);
		Transactions.AddChild(label);

		var tween = CreateTween();
		var endPosition = label.Position + Vector2.Down * 25;

		tween.TweenProperty(label, "position", endPosition, 1.5f).SetTrans(Tween.TransitionType.Circ).SetEase(Tween.EaseType.Out);
		tween.TweenProperty(label, "modulate:a", 0.0f, 1.0f);
		tween.TweenCallback(Callable.From(() =>
			{
				label.QueueFree();
				tween.Kill();
			}));

		tween.Play();
	}

	public void AddNotification(Label label, double delay = 5)
	{
		if (Notifications is null) return;

		Notifications.AddChild(label);

		var tween = CreateTween();

		tween.TweenProperty(label, "modulate:a", 0.0f, 1.0f).SetDelay(delay);
		tween.TweenCallback(Callable.From(() =>
			{
				label.QueueFree();
				tween.Kill();
			}));

		tween.Play();
	}

	private void OnPlayerInteracting(List<Button> actions)
	{
		foreach (var action in actions)
		{
			Actions?.AddChild(action);
		}
	}

	private void OnPlayerStoppedInteracting(List<Button> actions)
	{
		foreach (var action in actions)
		{
			Actions?.RemoveChild(action);
		}
	}

	private void OnGameTimeChanged(int time)
	{
		SetTimeAndDay(time);
	}

	private void OnTransactionComplete(Transaction transaction)
	{
		SetMoney(transaction.AmountAfterTransaction);
		AddTransaction(transaction.TransactionAmount);
	}

	private void OnNotifyPlayer(string message)
	{
		var label = new Label();
		label.Text = message;
		AddNotification(label);
	}
}
