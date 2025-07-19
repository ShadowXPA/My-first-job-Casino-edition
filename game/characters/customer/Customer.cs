using Godot;
using ProjectGJ.Props.Customer;
using ProjectGJ.Props.Slots;
using ProjectGJ.Scripts;
using ProjectGJ.Scripts.Items;
using System;
using System.Linq;
using System.Text;

namespace ProjectGJ.Characters.Customer;

public partial class Customer : CharacterBody2D
{
	[ExportGroup("Movement")]
	[Export]
	public float Speed { get; set; } = 100.0f;

	public CustomerItem? CustomerItem { get; private set; }
	public Label? CustomerName { get; private set; }

	private AnimatedSprite2D? _animatedSprite;
	private NavigationAgent2D? _navigationAgent;
	private Vector2? _lastDirection;

	private Node2D? _target;
	private CustomerActivity? _currentActivity;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("%Sprite");
		_navigationAgent = GetNode<NavigationAgent2D>("%NavigationAgent");
		CustomerName = GetNode<Label>("%Name");

		_navigationAgent.VelocityComputed += OnVelocityComputed;
		SignalBus.GameTimeChanged += OnTimeChanged;
	}

	public override void _ExitTree()
	{
		if (_navigationAgent is not null)
			_navigationAgent.VelocityComputed -= OnVelocityComputed;
		SignalBus.GameTimeChanged -= OnTimeChanged;
	}

	public void SetCustomerItem(CustomerItem customerItem)
	{
		CustomerItem = customerItem;
		SetCharacter(CustomerItem.Resource!);

		if (CustomerName is not null)
		{
			CustomerName.Text = customerItem.Name;
		}
	}

	public void SetCharacter(string resource)
	{
		if (_animatedSprite is null) return;

		_animatedSprite.SpriteFrames = GD.Load<SpriteFrames>(resource);
	}

	public void NextActivity()
	{
		if (CustomerItem is null || CustomerItem.Activities.Count == 0) return;

		_currentActivity = CustomerItem.Activities[0];
		CustomerItem.Activities.RemoveAt(0);
		var activityName = _currentActivity.Activity.ToString().ToLower();
		var nodes = GetTree().GetNodesInGroup(activityName);
		var numNodes = nodes.Count;

		while (numNodes != 0)
		{
			switch (_currentActivity.Activity)
			{
				case ActivityType.Home:
					_target = (CustomerHome)nodes.PickRandom();
					return;
				case ActivityType.Bar:
				case ActivityType.Poker:
				case ActivityType.Roulette:
					{
						var workerStation = (WorkerStation)nodes.PickRandom();
						var potentialTarget = workerStation.TryOccupyTable(this);

						if (potentialTarget is not null)
						{
							_target = potentialTarget;
							return;
						}

						nodes.Remove(potentialTarget);
					}
					break;
				case ActivityType.Slots:
					{
						var potentialTarget = (Slots)nodes.PickRandom();

						if (potentialTarget.CanOccupy)
						{
							potentialTarget.Occupy(this);
							_target = potentialTarget;
							return;
						}

						nodes.Remove(potentialTarget);
					}
					break;
				default:
					return;
			}

			numNodes--;
		}

		NextActivity();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_target is null || _navigationAgent is null) return;

		if (_navigationAgent.TargetPosition != _target.GlobalPosition)
		{
			_navigationAgent.TargetPosition = _target.GlobalPosition;
		}

		if (_navigationAgent.IsNavigationFinished())
		{
			_lastDirection = GlobalPosition.DirectionTo(_target.GlobalPosition).ToCardinalDirection();
			PlayAnimation(Vector2.Zero);

			if (_target is Slots slots)
			{
				// TODO: make this better...
				slots.Gamble();
			}

			_target = null;

			if (_currentActivity is not null && _currentActivity.Activity == ActivityType.Home)
			{
				QueueFree();
			}

			return;
		}

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

	private void OnVelocityComputed(Vector2 safeVelocity)
	{
		Velocity = safeVelocity;
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

	private void OnTimeChanged(int time)
	{
		if (_currentActivity is not null && _currentActivity.Activity != ActivityType.Home && _target is null)
		{
			_currentActivity.TimeLeft--;

			if (_currentActivity.TimeLeft == 0)
			{
				NextActivity();
			}
		}
	}
}
