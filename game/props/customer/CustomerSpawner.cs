using Godot;

namespace ProjectGJ.Props.Customer;

public partial class CustomerSpawner : Node2D
{
    public override void _Ready()
    {
        GetNode<Sprite2D>("%Sprite").Visible = false;
    }
}
