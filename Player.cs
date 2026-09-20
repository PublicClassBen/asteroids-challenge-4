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
	public float Acceleration { get; set; } = 400.0f;

	[Export]
	public float CoastingDrag { get; set; } = 0.35f;

	[Export]
	public PackedScene BulletScene { get; set; }

	private Marker2D _muzzle;

	private AudioStreamPlayer _shootSound;
	private AudioStreamPlayer _throttleSound;
	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_muzzle = GetNode<Marker2D>("Muzzle");
		_shootSound = GetNode<AudioStreamPlayer>("shootSound");
		_throttleSound = GetNode<AudioStreamPlayer>("throttleSound");
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
			if (!_throttleSound.Playing)
			{
				_throttleSound.Play();
			}
			_sprite.Animation = "throttle";
		}
		else
		{
			Velocity *= Mathf.Exp(-CoastingDrag * delta);
			_sprite.Animation = "no_throttle";
			_throttleSound.Stop();

			if (Velocity.Length() < 1.0f)
			{
				Velocity = Vector2.Zero;
			}
		}

		if (Input.IsActionJustPressed("fire"))
		{
			Shoot();
		}

		Velocity = Velocity.LimitLength(MaxSpeed);
		MoveAndSlide();
	}

	private void Shoot()
	{
		Bullet bullet = BulletScene.Instantiate<Bullet>();
		bullet.SetDirection(-GlobalTransform.Y);
		GetParent().AddChild(bullet);
		bullet.GlobalPosition = _muzzle.GlobalPosition;
		_shootSound.Play();
	}

}
