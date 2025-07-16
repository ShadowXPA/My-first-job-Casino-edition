using Godot;
using ProjectGJ.Components.Interactable;
using ProjectGJ.Scripts;
using ProjectGJ.Scripts.Items;
using System;
using System.Collections.Generic;

namespace ProjectGJ.Props.Statue;

public partial class Statue : StaticBody2D
{
	private Sprite2D? _statue;
	private PlayerInteractable? _playerInteractable;

	public override void _Ready()
	{
		_statue = GetNode<Sprite2D>("%Sprite");
		_playerInteractable = GetNode<PlayerInteractable>("%PlayerInteractable");

		var changeStatueButton = Utils.CreateActionButton("Change", OnChangeStatuePressed);

		_playerInteractable.Actions.Add(changeStatueButton);
		_playerInteractable.ExitAction += () => SignalBus.BroadcastStatueMenuButtonPressed(false);

		SignalBus.PlayerSelectedStatue += SetStatue;
	}

	public override void _ExitTree()
	{
		SignalBus.PlayerSelectedStatue -= SetStatue;
	}

	public void SetStatue(StatueItem item)
	{
		if (_statue is null) return;

		var texture = GD.Load<CompressedTexture2D>(item.Resource);
		var size = texture.GetSize();

		_statue.Texture = texture;
		_statue.Offset = Vector2.Up * (size.Y / 2);
	}

	private void OnChangeStatuePressed()
	{
		SignalBus.BroadcastStatueMenuButtonPressed();
	}
}
