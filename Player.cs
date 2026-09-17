using System.Diagnostics.CodeAnalysis;
using Godot;

public partial class Player : CharacterBody2D
{
	private AnimatedSprite2D _sprite;

	[Export]
	public float MaxSpeed { get; set; } = 400.0f;

	[Export]
	public float RotationSpeed { get; set; } = 3.0f;

	[Export]
	public float Acceleration { get; set; } = 250.0f;

	[Export]
	public float CoastingDrag { get; set; } = 0.35f;
	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		handleMovement(dt);
	}

	private void handleMovement(float delta)
	{
		float rotationDirection = Input.GetAxis("rotate_left", "rotate_right");
		Rotation += rotationDirection * RotationSpeed * delta;
		bool thrusting = Input.IsActionPressed("throttle");
		if (thrusting)
		{
			Velocity += -Transform.Y * Acceleration * delta;
		}
		else
		{
			Velocity *= Mathf.Exp(-CoastingDrag * delta);

			if (Velocity.Length() < 1.0f)
			{
				Velocity = Vector2.Zero;
			}
		}

		Velocity.LimitLength(MaxSpeed);
		MoveAndSlide();
	}
}
