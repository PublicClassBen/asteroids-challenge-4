using Godot;
using System;

public partial class Bullet : Area2D
{

	[Export]
	public float Speed { get; set; } = 600f;

	[Export]
	public float Radius { get; set; } = 2f;

	[Export]
	public Color BulletColor { get; set; } = Colors.White;

	private Vector2 _direction;
	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		QueueRedraw();
		var notifier = GetNode<VisibleOnScreenNotifier2D>("ScreenNotifier");
		notifier.ScreenExited += QueueFree;
	}
	public override void _Draw()
	{
		DrawCircle(Vector2.Zero, Radius * 2f, new Color(1f, 1f, 1f, 0.2f));
		DrawCircle(Vector2.Zero, Radius, BulletColor);
	}

	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition += _direction * Speed * (float)delta;
	}


	public void SetDirection(Vector2 direction)
	{
		_direction = direction.Normalized();
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area is Asteroid asteroid)
		{
			Callable.From(asteroid.Split).CallDeferred();
			QueueFree();
		}
	}
}
