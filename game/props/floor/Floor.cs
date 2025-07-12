using Godot;
using System;

[GlobalClass]
[Tool]
public partial class Floor : Node2D
{
    private static readonly string FLOOR_SPRITE_RESOURCE_PATH = "res://props/floor";

    [Export]
    public Material Type
    {
        get => _floorType; set
        {
            _floorType = value;
            UpdateTexture();
        }
    }
    [Export]
    public Orientation Orientation
    {
        get => _floorOrientation; set
        {
            _floorOrientation = value;
            UpdateTexture();
        }
    }

    private Material _floorType;
    private Orientation _floorOrientation;
    private Sprite2D? _floorSprite;

    public override void _Ready()
    {
        _floorSprite = GetNode<Sprite2D>("%Sprite");
        UpdateTexture();
    }

    private void UpdateTexture()
    {
        if (_floorSprite is null) return;
        var floor = Type.ToResourceString();
        _floorSprite.Texture = GD.Load<Texture2D>($"{FLOOR_SPRITE_RESOURCE_PATH}/{floor}/{floor}_{Orientation.ToResourceString()}.png");
    }
}
