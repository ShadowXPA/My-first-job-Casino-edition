using Godot;
using System;

public partial class ShopMenu : Control
{
	private Button? _exitButton;

	public override void _Ready()
	{
		SignalBus.ShopButtonPressed += OnShopPressed;
		_exitButton = GetNode<Button>("%Exit");
		_exitButton.Pressed += () => Visible = false;
	}

	public override void _ExitTree()
	{
		SignalBus.ShopButtonPressed -= OnShopPressed;
    }

	private void OnShopPressed()
	{
		Visible = true;
	}
}
