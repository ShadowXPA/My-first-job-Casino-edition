using Godot;
using ProjectGJ.Components.Interactable;
using ProjectGJ.Props.Bar;
using ProjectGJ.Props.Blackjack;
using ProjectGJ.Props.Customer;
using ProjectGJ.Props.Roulette;
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
	public Control? Transactions { get; private set; }

	private AnimatedSprite2D? _animatedSprite;
	private PlayerInteractable? _playerInteractable;
	private NavigationAgent2D? _navigationAgent;
	private Vector2? _lastDirection;

	private Node2D? _target;
	private CustomerActivity? _currentActivity;
	private Node2D? _currentStation;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("%Sprite");
		_playerInteractable = GetNode<PlayerInteractable>("%PlayerInteractable");
		_navigationAgent = GetNode<NavigationAgent2D>("%NavigationAgent");
		CustomerName = GetNode<Label>("%Name");
		Transactions = GetNode<Control>("%Transactions");

		_navigationAgent.VelocityComputed += OnVelocityComputed;
		SignalBus.GameTimeChanged += OnTimeChanged;
	}

	public override void _ExitTree()
	{
		if (_navigationAgent is not null)
			_navigationAgent.VelocityComputed -= OnVelocityComputed;
		SignalBus.GameTimeChanged -= OnTimeChanged;
	}

	public void AddTransaction(int amount)
	{
		if (Transactions is null) return;

		var label = new Label();
		label.AddThemeFontSizeOverride("font_size", 12);
		label.Text = $"{(amount < 0 ? "-" : "+")}${Mathf.Abs(amount)}";
		label.Modulate = new Color(amount < 0 ? 1 : 0, amount > 0 ? 1 : 0, amount == 0 ? 1 : 0, 1);
		Transactions.AddChild(label);
		label.Position = new Vector2 { X = -label.Size.X / 2, Y = -label.Size.Y / 2 };

		var tween = CreateTween();
		var endPosition = label.Position + Vector2.Up * 25;

		tween.TweenProperty(label, "position", endPosition, 1.5f).SetTrans(Tween.TransitionType.Circ).SetEase(Tween.EaseType.Out);
		tween.TweenProperty(label, "modulate:a", 0.0f, 1.0f);
		tween.TweenCallback(Callable.From(() =>
			{
				label.QueueFree();
				tween.Kill();
			}));

		tween.Play();
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
				case ActivityType.Blackjack:
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

			_currentStation = _target;
			_target = null;

			if (_currentActivity is not null)
			{
				switch (_currentActivity.Activity)
				{
					case ActivityType.Home:
						QueueFree();
						break;
					case ActivityType.Slots:
						if (_currentStation is Slots slots)
						{
							slots.Gamble();
						}
						break;
					case ActivityType.Bar:
						_currentStation = (Node2D)_currentStation.GetParent().GetParent();
						if (_currentStation is Bar bar)
						{
							bar.Drink(this);
						}
						break;
					case ActivityType.Roulette:
					case ActivityType.Blackjack:
						_currentStation = (Node2D)_currentStation.GetParent().GetParent();
						if (_currentStation is Roulette roulette)
						{
							roulette.Play(this);
						}
						else if (_currentStation is Blackjack blackjack)
						{
							blackjack.Play(this);
						}
						break;
				}
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
				switch (_currentActivity.Activity)
				{
					case ActivityType.Slots:
						if (_currentStation is Slots slots)
						{
							slots.Occupy(null);
						}
						break;
					case ActivityType.Bar:
						if (_currentStation is Bar bar)
						{
							bar.LeaveTable(this);
						}
						break;
					case ActivityType.Roulette:
					case ActivityType.Blackjack:
						if (_currentStation is Roulette roulette)
						{
							roulette.LeaveTable(this);
						}
						else if (_currentStation is Blackjack blackjack)
						{
							blackjack.LeaveTable(this);
						}
						break;
				}

				_currentStation = null;
				NextActivity();
			}
		}
	}
}
