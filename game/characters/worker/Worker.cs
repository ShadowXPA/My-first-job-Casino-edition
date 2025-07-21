using System.Text;
using Godot;
using ProjectGJ.Scripts.Items;

namespace ProjectGJ.Characters.Worker;

public partial class Worker : CharacterBody2D
{
	public WorkerItem? WorkerItem { get; private set; }
	public Label? WorkerName { get; private set; }

	private AnimatedSprite2D? _animatedSprite;
	private Vector2? _lastDirection;

	public override void _Ready()
	{
		_lastDirection = Vector2.Down;
		_animatedSprite = GetNode<AnimatedSprite2D>("%Sprite");
		WorkerName = GetNode<Label>("%Name");
	}

	public void SetWorker(WorkerItem workerItem)
	{
		WorkerItem = workerItem;
		SetCharacter(workerItem.Resource!);

		if (WorkerName is not null)
		{
			WorkerName.Text = workerItem.Name;
		}
	}

	public void SetCharacter(string resource)
	{
		if (_animatedSprite is null) return;

		_animatedSprite.SpriteFrames = GD.Load<SpriteFrames>(resource);
	}

	public void PlayAnimation(Vector2 direction, float animationSpeed = 1.0f)
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
