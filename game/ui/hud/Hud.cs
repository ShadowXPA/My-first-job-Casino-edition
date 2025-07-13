using Godot;
using System;
using System.Collections.Generic;

public partial class Hud : Control
{
	public Label? Time;
	public Label? Day;
	public Label? Money;
	public VBoxContainer? Actions;

	public override void _Ready()
	{
		Time = GetNode<Label>("%Time");
		Day = GetNode<Label>("%Day");
		Money = GetNode<Label>("%Money");
		Actions = GetNode<VBoxContainer>("%Actions");
		SignalBus.PlayerInteracting += PlayerInteracting;
		SignalBus.PlayerStoppedInteracting += PlayerStoppedInteracting;
	}

	public override void _ExitTree()
	{
		SignalBus.PlayerInteracting -= PlayerInteracting;
		SignalBus.PlayerStoppedInteracting -= PlayerStoppedInteracting;
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
