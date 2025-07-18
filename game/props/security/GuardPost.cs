using Godot;
using ProjectGJ.Characters.Worker;
using System;

namespace ProjectGJ.Props.Security;

public partial class GuardPost : Node2D
{
	public Worker? Worker { get; private set; }

	public override void _Ready()
	{
		GetNode<Sprite2D>("%Sprite").Visible = false;
	}

	public void SetWorker(Worker? worker)
	{
		Worker = worker;
	}
}
