using Godot;
using System;
using System.Collections.Generic;

public partial class Hud : Control
{
	public Label? Time { get; private set; }
	public Label? Day { get; private set; }
	public Label? Money { get; private set; }
	public Control? Transactions { get; private set; }
	public VBoxContainer? Actions { get; private set; }

	public override void _Ready()
	{
		Time = GetNode<Label>("%Time");
		Day = GetNode<Label>("%Day");
		Money = GetNode<Label>("%Money");
		Transactions = GetNode<Control>("%Transactions");
		Actions = GetNode<VBoxContainer>("%Actions");
		SignalBus.PlayerInteracting += PlayerInteracting;
		SignalBus.PlayerStoppedInteracting += PlayerStoppedInteracting;
	}

	public override void _ExitTree()
	{
		SignalBus.PlayerInteracting -= PlayerInteracting;
		SignalBus.PlayerStoppedInteracting -= PlayerStoppedInteracting;
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

		Money.Text = $"${money}";
	}

	public void AddTransaction(int amount)
	{
		if (Transactions is null) return;

		var label = new Label();
		label.Text = $"{(amount < 0 ? "-" : "+")}${Mathf.Abs(amount)}";
		label.Modulate = new Color(amount < 0 ? 1 : 0, amount > 0 ? 1 : 0, amount == 0 ? 1 : 0, 1);
		Transactions.AddChild(label);
		label.HorizontalAlignment = HorizontalAlignment.Right;

		var tween = CreateTween();
		var endPosition = label.Position + Vector2.Down * 50;

		tween.TweenProperty(label, "position", endPosition, 1.5f).SetTrans(Tween.TransitionType.Circ).SetEase(Tween.EaseType.Out);
		tween.Parallel().TweenProperty(label, "modulate:a", 0.0f, 1.5f);
		tween.TweenCallback(Callable.From(() =>
			{
				label.QueueFree();
				tween.Kill();
			}));

		tween.Play();
	}

	private void PlayerInteracting(List<Button> actions)
	{
		foreach (var action in actions)
		{
			Actions?.AddChild(action);
		}
	}

	private void PlayerStoppedInteracting(List<Button> actions)
	{
		foreach (var action in actions)
		{
			Actions?.RemoveChild(action);
		}
	}
}
