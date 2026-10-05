using Godot;
using System;

public partial class Asteroids : Node2D
{
	private Ui _ui;

	private AsteroidManager _asteroidManager;

	private Player _player;

	private bool _gameStarted;

	private int _score = 0;
	private int _currentWave = 1;
	private int _lives = 3;
	public override void _Ready()
	{
		_ui = GetNode<Ui>("ui");
		_asteroidManager = GetNode<AsteroidManager>("asteroid_manager");
		_player = GetNode<Player>("player");

		_asteroidManager.AsteroidDestroyed += OnAsteroidDestroyed;
		_asteroidManager.WaveCleared += OnWaveCleared;
		_ui.UpdateScore(_score);

		ProcessMode = ProcessModeEnum.Always;

		_player.ProcessMode = ProcessModeEnum.Pausable;
		_asteroidManager.ProcessMode = ProcessModeEnum.Pausable;
		_player.OnPlayerCollision += OnPlayerHit;

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
		ResetGame();
		GetTree().Paused = false;
		_asteroidManager.NextWave(_currentWave);
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

	private async void OnWaveCleared()
	{
		if (_lives <= 0 || !_gameStarted)
			return;
		_ui.ShowInfo($"Wave {_currentWave} Completed. More Asteroids Incoming!");

		await ToSignal(
			GetTree().CreateTimer(5.0),
			SceneTreeTimer.SignalName.Timeout
		);

		_currentWave++;
		_ui.HideInfo();
		_asteroidManager.NextWave(_currentWave);
	}

	private async void OnPlayerHit()
	{
		_lives--;
		if (_lives == 0)
		{
			_ui.UpdateLives(_lives);
			GetTree().Paused = true;
			_ui.ShowInfo("Game Over.");
			await ToSignal(
			GetTree().CreateTimer(5.0),
			SceneTreeTimer.SignalName.Timeout
			);
			_gameStarted = false;
			_asteroidManager.removeAsteroids();
			_ui.ShowInfo("Press Enter to Start");
		}
		else
		{
			_ui.UpdateLives(_lives);
		}
	}

	private void ResetGame()
	{
		_lives = 3;
		_score = 0;
		_currentWave = 0;
		_ui.UpdateScore(_score);
		_ui.UpdateLives(_lives);
	}
}
