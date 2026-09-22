using Godot;
using System;

public partial class AsteroidManager : Node2D
{
	private Marker2D[] _asteroid_spawns;
	private int current_wave = 0;

	private int _asteroidsRemaining = 0;

	[Export]
	public PackedScene AsteroidScene { get; set; }

	[Signal]
	public delegate void WaveClearedEventHandler();

	[Signal]
	public delegate void AsteroidDestroyedEventHandler(int asteroidType);

	public override void _Ready()
	{
		_asteroid_spawns = new Marker2D[28];

		for (int i = 0; i < _asteroid_spawns.Length; i++)
		{
			_asteroid_spawns[i] = GetNode<Marker2D>($"asteroidSpawn{i + 1}");

			if (_asteroid_spawns[i] == null)
			{
				GD.PrintErr($"index {i} could not be found");
			}
		}
		ChildEnteredTree += OnChildEnteredTree;
		ChildExitingTree += OnChildExitingTree;
		spawnAsteroids();
	}


	private void spawnAsteroids()
	{
		int numberOfAsteroids = current_wave + 4;

		for (int i = 0; i < numberOfAsteroids; i++)
		{
			int index = GD.RandRange(0, 27);
			Asteroid asteroid = AsteroidScene.Instantiate<Asteroid>();
			AddChild(asteroid);
			asteroid.GlobalPosition = _asteroid_spawns[index].GlobalPosition;
			asteroid.SetAsteroidProperties(AsteroidType.Large);
		}
	}

	private void OnChildEnteredTree(Node child)
	{
		if (child is Asteroid asteroid)
		{
			_asteroidsRemaining++;
			asteroid.AsteroidDestroyed += OnAsteroidDestroyed;
			
		}
	}

	private void OnChildExitingTree(Node child)
	{
		if (child is not Asteroid)
		{
			return;
		}

		_asteroidsRemaining--;

		if (_asteroidsRemaining == 0)
		{
			GD.Print($"Wave {current_wave} completed!");
			EmitSignal(SignalName.WaveCleared);
		}
	}

	public void NextWave()
	{
		current_wave++;
		spawnAsteroids();
	}

	private void OnAsteroidDestroyed(int asteroidType)
	{
		EmitSignal(SignalName.AsteroidDestroyed, asteroidType);
	}

}
