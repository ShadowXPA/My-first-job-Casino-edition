using Godot;
using ProjectGJ.Scripts;

namespace ProjectGJ.Props.Roulette;

public partial class Roulette : WorkerStation
{
	private AnimatedSprite2D? _animatedSprite;

	public override void _Ready()
	{
		base._Ready();
		_animatedSprite = GetNode<AnimatedSprite2D>("%RouletteWheel");
	}

	public void StartWheel()
	{
		if (_animatedSprite is null) return;

		_animatedSprite.Play("rolling");
	}

	public void StopWheel()
	{
		if (_animatedSprite is null) return;

		_animatedSprite.Play("default");
	}
}
