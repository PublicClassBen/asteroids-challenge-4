using Godot;
using System;

public partial class Asteroids : Node2D
{
	private Ui _ui;

	private AsteroidManager _asteroidManager;

	private Player _player;

	private bool _gameStarted;

	private int _score = 0;
	public override void _Ready()
	{
		_ui = GetNode<Ui>("ui");
		_asteroidManager = GetNode<AsteroidManager>("asteroid_manager");
		_player = GetNode<Player>("player");

		_asteroidManager.AsteroidDestroyed += OnAsteroidDestroyed;
		_ui.UpdateScore(_score);

		ProcessMode = ProcessModeEnum.Always;

		_player.ProcessMode = ProcessModeEnum.Pausable;
		_asteroidManager.ProcessMode = ProcessModeEnum.Pausable;

		GetTree().Paused = true;

		_ui.ShowInfo("Press Enter to Start");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _UnhandledInput(InputEvent @event)
	{
		if (!_gameStarted && @event.IsActionPressed("start_game"))
		{
			StartGame();
		}
	}

	private void StartGame()
	{
		_gameStarted = true;
		_ui.HideInfo();
		GetTree().Paused = false;
	}

	private void OnAsteroidDestroyed(int asteroidTypeValue)
	{
		var asteroidType = (AsteroidType)asteroidTypeValue;

		int points = asteroidType switch
		{
			AsteroidType.Large => 20,
			AsteroidType.Medium => 50,
			AsteroidType.Small => 100,
			_ => 0
		};

		_score += points;
		_ui.UpdateScore(_score);
	}
}
