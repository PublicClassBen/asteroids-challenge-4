using Godot;
using System;

public partial class Asteroid : Area2D
{
	[Export]
	public float RotationSpeed { get; set; } = 0.25f;

	[Export]
	public float Speed { get; set; } = 100f;

	private Vector2 _direction;
	private int _rotationDirection;

	private CollisionShape2D _collisionLarge;
	private CollisionShape2D _collisionMedium;
	private CollisionShape2D _collisionSmall;

	private ScreenWarp _screenWrapLarge;
	private ScreenWarp _screenWrapMedium;
	private ScreenWarp _screenWrapSmall;

	private AsteroidType _asteroidType;

	private AudioStreamPlayer2D _asteroidHitSound;

	private Sprite2D _sprite;

	[Signal]
	public delegate void AsteroidDestroyedEventHandler(int asteroidType);
	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite2D");
		_collisionLarge = GetNode<CollisionShape2D>("collision_large");
		_collisionMedium = GetNode<CollisionShape2D>("collision_medium");
		_collisionSmall = GetNode<CollisionShape2D>("collision_small");
		_screenWrapLarge = GetNode<ScreenWarp>("ScreenWarp_large");
		_screenWrapMedium = GetNode<ScreenWarp>("ScreenWarp_medium");
		_screenWrapSmall = GetNode<ScreenWarp>("ScreenWarp_small");
		GD.RandRange(0, 1);
		_rotationDirection = GD.RandRange(0, 1) == 0 ? -1 : 1;

		// Choose a random angle from 0 to 360 degrees.
		float randomAngle = GD.Randf() * Mathf.Tau;

		// Convert the angle into a direction vector.
		_direction = Vector2.Right.Rotated(randomAngle);

		_asteroidHitSound = GetNode<AudioStreamPlayer2D>("asteroidHitSound");
		SetAsteroidProperties(AsteroidType.Large); // Default to Large, can be changed later
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Rotation += RotationSpeed * _rotationDirection * (float)delta;
		Position += _direction * Speed * (float)delta;
	}

	public void SetAsteroidProperties(AsteroidType type)
	{
		_asteroidType = type;
		switch (type)
		{
			case AsteroidType.Large:
				_collisionLarge.Disabled = false;
				_collisionMedium.Disabled = true;
				_collisionSmall.Disabled = true;

				_screenWrapLarge.Visible = true;
				_screenWrapMedium.Visible = false;
				_screenWrapSmall.Visible = false;
				_sprite.Frame = 0; // Assuming frame 0 is for Large asteroids
				break;

			case AsteroidType.Medium:
				_collisionLarge.Disabled = true;
				_collisionMedium.Disabled = false;
				_collisionSmall.Disabled = true;

				_screenWrapLarge.Visible = false;
				_screenWrapMedium.Visible = true;
				_screenWrapSmall.Visible = false;
				_sprite.Frame = 1; // Assuming frame 1 is for Medium asteroids
				break;

			case AsteroidType.Small:
				_collisionLarge.Disabled = true;
				_collisionMedium.Disabled = true;
				_collisionSmall.Disabled = false;

				_screenWrapLarge.Visible = false;
				_screenWrapMedium.Visible = false;
				_screenWrapSmall.Visible = true;
				_sprite.Frame = 2; // Assuming frame 2 is for Small asteroids
				break;

			default:
				GD.PrintErr("Invalid asteroid type: " + type);
				break;
		}
	}

	public void Split()
	{
		EmitSignal(SignalName.AsteroidDestroyed, (int) _asteroidType);
		var sound = _asteroidHitSound;

		// Move the sound under Game so it survives the asteroid.
		sound.Reparent(GetParent());

		// Remove the sound player once playback finishes.
		sound.Finished += sound.QueueFree;

		sound.Play();
		if (_asteroidType == AsteroidType.Large)
		{
			// Create two medium asteroids
			CreateAsteroid(AsteroidType.Medium);
			CreateAsteroid(AsteroidType.Medium);
		}
		else if (_asteroidType == AsteroidType.Medium)
		{
			// Create two small asteroids
			CreateAsteroid(AsteroidType.Small);
			CreateAsteroid(AsteroidType.Small);
		}
		QueueFree(); // Remove the current asteroid
	}

	private void CreateAsteroid(AsteroidType type)
	{
		var asteroidScene = GD.Load<PackedScene>("res://asteroid.tscn");
		var newAsteroid = asteroidScene.Instantiate<Asteroid>();
		newAsteroid.Position = Position; // Set the position to the current asteroid's position
		GetParent().AddChild(newAsteroid); // Add the new asteroid to the same parent
										   // Adding it to the tree runs _Ready() and initializes its child-node references.
		newAsteroid.SetAsteroidProperties(type);
	}
}
