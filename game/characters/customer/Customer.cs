using Godot;
using ProjectGJ.Props.Slots;
using ProjectGJ.Scripts;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace ProjectGJ.Characters.Customer;

public partial class Customer : CharacterBody2D
{
	[ExportGroup("Movement")]
	[Export]
	public float Speed { get; set; } = 200.0f;
	// TODO: type of customer, stats, money, activities

	private AnimatedSprite2D? _animatedSprite;
	private NavigationAgent2D? _navigationAgent;
	private Vector2? _lastDirection;

	private Node2D? _target;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("%Sprite");
		_navigationAgent = GetNode<NavigationAgent2D>("%NavigationAgent");

		_navigationAgent.VelocityComputed += OnVelocityComputed;
	}

	public override void _ExitTree()
	{
		if (_navigationAgent is not null)
			_navigationAgent.VelocityComputed -= OnVelocityComputed;
	}

	public override void _PhysicsProcess(double delta)
	{
		_target ??= (Node2D?)GetTree().GetNodesInGroup("slots").Where(n => ((Slots?)n)?.Customer is null)
				.ElementAt(GD.RandRange(0, GetTree().GetNodesInGroup("slots").Where(n => ((Slots?) n)?.Customer is null).Count() - 1));

		if (_target is not null && _navigationAgent is not null && _navigationAgent.TargetPosition != _target.GlobalPosition)
		{
			((Slots)_target).Occupy(this);
			_target.TopLevel = true;
			_navigationAgent.TargetPosition = _target.GlobalPosition;
		}

		if (_target is not null && _navigationAgent is not null && _navigationAgent.IsNavigationFinished())
		{
			_lastDirection = GlobalPosition.DirectionTo(_target.GlobalPosition).ToCardinalDirection();
			PlayAnimation(Vector2.Zero);
			return;
		}

		if (_navigationAgent is not null && !_navigationAgent.IsNavigationFinished())
		{
			var nextPos = _navigationAgent.GetNextPathPosition();
			var direction = GlobalPosition.DirectionTo(nextPos).ToCardinalDirection();
			var newVelocity = direction * Speed;

			if (_navigationAgent.AvoidanceEnabled)
				_navigationAgent.Velocity = newVelocity;
			else
				OnVelocityComputed(newVelocity);

			MoveAndSlide();
			PlayAnimation(direction);
			_lastDirection = direction;
		}
	}

	public void OnVelocityComputed(Vector2 safeVelocity)
	{
		Velocity = safeVelocity;
	}

	public void SetCharacter(string resource)
	{
		if (_animatedSprite is null) return;

		_animatedSprite.SpriteFrames = GD.Load<SpriteFrames>(resource);
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
}
