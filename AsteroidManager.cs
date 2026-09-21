using Godot;
using System;

public partial class AsteroidManager : Node2D
{
	private Marker2D[] _asteroid_spawns;
	private int current_wave = 0;

	[Export]
	public PackedScene AsteroidScene { get; set; }

    public override void _Ready()
    {
        _asteroid_spawns = new Marker2D[28];

		for(int i = 0; i < _asteroid_spawns.Length; i++)
		{
			_asteroid_spawns[i] = GetNode<Marker2D>($"asteroidSpawn{i + 1}");

			if(_asteroid_spawns[i] == null)
			{
				GD.PrintErr($"index {i} could not be found");
			}
		}
		spawnAsteroids();
    }


	private void spawnAsteroids()
	{
		int numberOfAsteroids = current_wave + 4;
		
		for(int i = 0; i < numberOfAsteroids; i++)
		{
			int index = GD.RandRange(0, 27);
			Asteroid asteroid = AsteroidScene.Instantiate<Asteroid>();
			AddChild(asteroid);
			asteroid.GlobalPosition = _asteroid_spawns[index].GlobalPosition;
			asteroid.SetAsteroidProperties(AsteroidType.Large);
		}
	}

}
