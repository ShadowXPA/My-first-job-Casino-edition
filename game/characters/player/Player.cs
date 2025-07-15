using Godot;
using System;
using System.Text;

public partial class Player : CharacterBody2D
{
	[ExportGroup("Movement")]
	[Export]
	public float Speed { get; set; } = 200.0f;
	[Export]
	public float SprintMultiplier { get; set; } = .75f;

	private AnimatedSprite2D? _animatedSprite;
	private Vector2? _lastDirection;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("%Sprite");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 rawInput = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Vector2 direction = Utils.GetCardinalDirection(rawInput);
		var sprinting = Input.IsActionPressed("sprint") ? 1 : 0;
		var speedModifier = SprintMultiplier * sprinting + 1.0f;

		Velocity = direction * Speed * speedModifier;
		MoveAndSlide();

		PlayAnimation(direction, speedModifier);
		_lastDirection = direction;
	}

	private void PlayAnimation(Vector2 direction, float animationSpeed = 1.0f)
	{
		if (_animatedSprite is null) return;

		var str = new StringBuilder();

		if (direction == Vector2.Zero)
		{
			str.Append("idle_");
		}
		else
		{
			str.Append("walk_");
		}

		if (direction == Vector2.Up || _lastDirection == Vector2.Up)
		{
			str.Append("up");
		}
		else if (direction == Vector2.Right || _lastDirection == Vector2.Right)
		{
			str.Append("right");
		}
		else if (direction == Vector2.Down || _lastDirection == Vector2.Down)
		{
			str.Append("down");
		}
		else if (direction == Vector2.Left || _lastDirection == Vector2.Left)
		{
			str.Append("left");
		}
		else
		{
			return;
		}

		_animatedSprite.SpeedScale = animationSpeed;
		_animatedSprite.Play(str.ToString());
	}

	// private void Move(Vector2 direction)
	// {
	// 	GlobalPosition += direction * Utils.TILE_SIZE;

	// 	if (direction == Vector2.Up)
	// 	{
	// 		_animatedSprite.Play("walk_up");
	// 	}
	// 	else if (direction == Vector2.Right)
	// 	{
	// 		_animatedSprite.Play("walk_right");
	// 	}
	// 	else if (direction == Vector2.Down)
	// 	{
	// 		_animatedSprite.Play("walk_down");
	// 	}
	// 	else if (direction == Vector2.Left)
	// 	{
	// 		_animatedSprite.Play("walk_left");
	// 	}

	// 	_lastDirection = direction;

	// 	// _animatedSprite!.GlobalPosition -= direction * Utils.TILE_SIZE;

	// 	// _nodePositionTween?.Kill();
	// 	// _nodePositionTween = CreateTween();
	// 	// _nodePositionTween.SetProcessMode(Tween.TweenProcessMode.Physics);
	// 	// _nodePositionTween.TweenProperty(_animatedSprite, "global_position", GlobalPosition, 0.1).SetTrans(Tween.TransitionType.Sine);
	// }
}
