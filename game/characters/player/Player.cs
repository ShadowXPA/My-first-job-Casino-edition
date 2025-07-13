using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[ExportGroup("Movement")]
	[Export]
	public float Speed { get; set; } = 300.0f;
	[Export]
	public float SprintMultiplier { get; set; } = .75f;

	public override void _PhysicsProcess(double delta)
	{
        ZIndex = (int)GlobalPosition.Y;
        Vector2 rawInput = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		var sprinting = Input.IsActionPressed("sprint") ? 1 : 0;

        Velocity = rawInput * Speed * (SprintMultiplier * sprinting + 1.0f);
		MoveAndSlide();
	}
}
