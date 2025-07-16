using Godot;
using ProjectGJ.Scripts.Items;
using System;

namespace ProjectGJ.Ui.Shop;

public partial class ShopItem : PanelContainer
{
	public TextureRect? Image { get; private set; }
	public Label? ItemName { get; private set; }
	public Label? ItemDescription { get; private set; }
	public Label? ItemPrice { get; private set; }
	public VBoxContainer? Actions { get; private set; }

	private Item? _item;

	public override void _Ready()
	{
		Image = GetNode<TextureRect>("%Image");
		ItemName = GetNode<Label>("%Name");
		ItemDescription = GetNode<Label>("%Description");
		ItemPrice = GetNode<Label>("%Price");
		Actions = GetNode<VBoxContainer>("%Actions");
	}

	public void SetPriceMultiplier(float priceMultiplier)
	{
		if (_item is null) return;

		_item.PriceMultiplier = priceMultiplier;

		if (ItemPrice is not null)
		{
			ItemPrice.Text = _item.PriceString();
		}
	}

	public void SetItem(Item item, params Control[] actions)
	{
		_item = item;

		if (ItemName is not null)
		{
			ItemName.Text = item.Name;
		}

		if (ItemDescription is not null)
		{
			ItemDescription.Text = item.Description ?? string.Empty;
		}

		if (ItemPrice is not null)
		{
			ItemPrice.Text = item.PriceString();
		}

		if (Image is not null && item.Resource is not null)
		{
			var loaded = GD.Load(item.Resource);

			if (loaded is SpriteFrames spriteFrames)
			{
				Image.Texture = spriteFrames.GetFrameTexture("idle_down", 0);
			}
			else if (loaded is CompressedTexture2D compressedTexture2D)
			{
				Image.Texture = compressedTexture2D;
			}
		}

		if (Actions is not null)
		{
			foreach (var action in actions)
			{
				Actions.AddChild(action);
			}
		}
	}
}
