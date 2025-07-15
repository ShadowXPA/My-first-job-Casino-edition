using Godot;
using System;

public partial class ShopItem : PanelContainer
{
	public TextureRect? Image { get; private set; }
	public Label? ItemName { get; private set; }
	public Label? ItemDescription { get; private set; }
	public VBoxContainer? Actions { get; private set; }

	public override void _Ready()
	{
		Image = GetNode<TextureRect>("%Image");
		ItemName = GetNode<Label>("%Name");
		ItemDescription = GetNode<Label>("%Description");
		Actions = GetNode<VBoxContainer>("%Actions");
	}

	public void SetItem(string name, string description, string? imageResource, params Control[] actions)
	{
		if (ItemName is not null)
		{
			ItemName.Text = name;
		}

		if (ItemDescription is not null)
		{
			ItemDescription.Text = description;
		}

		if (Image is not null && imageResource is not null)
		{
			Image.Texture = GD.Load<CompressedTexture2D>(imageResource);
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
