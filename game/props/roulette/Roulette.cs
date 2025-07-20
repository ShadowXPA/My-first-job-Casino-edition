using System.Collections.Generic;
using Godot;
using ProjectGJ.Scripts;

namespace ProjectGJ.Props.Roulette;

public partial class Roulette : WorkerStation
{
	private AnimatedSprite2D? _animatedSprite;
	private List<Characters.Customer.Customer> _customersPlaying = [];
	private bool _rolling;

	public override void _Ready()
	{
		base._Ready();
		_animatedSprite = GetNode<AnimatedSprite2D>("%RouletteWheel");
		SignalBus.GameTimeChanged += OnTimeChanged;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		SignalBus.GameTimeChanged -= OnTimeChanged;
	}

	public void Play(Characters.Customer.Customer customer)
	{
		if (Worker is null) return;

		_customersPlaying.Add(customer);
	}

	public override void LeaveTable(Characters.Customer.Customer customer)
	{
		base.LeaveTable(customer);
		_customersPlaying.Remove(customer);

		if (_customersPlaying.Count == 0)
		{
			StopWheel();
		}
	}

	private void OnTimeChanged(int time)
	{
		if (Worker is null || _customersPlaying.Count == 0) return;

		if (time % 5 == 0)
		{
			if (_rolling)
			{
				StopWheel();

				foreach (var customer in _customersPlaying)
					SignalBus.BroadcastCustomerGamblingRoulette(customer);
			}
			else
				StartWheel();
		}
	}

	private void StartWheel()
	{
		if (_animatedSprite is null) return;

		_rolling = true;
		_animatedSprite.Play("rolling");
	}

	private void StopWheel()
	{
		if (_animatedSprite is null) return;

		_rolling = false;
		_animatedSprite.Play("default");
	}
}
