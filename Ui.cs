using Godot;
using System;

public partial class Ui : CanvasLayer
{
	private Label _scoreLabel;
	private Label _livesLabel;
	private Label _infoLabel;

	public override void _Ready()
	{
		_scoreLabel = GetNode<Label>("score");
		_livesLabel = GetNode<Label>("lives");
		_infoLabel = GetNode<Label>("info");
	}

	public void UpdateScore(int score)
	{
		_scoreLabel.Text = $"Score: {score}";
	}

	public void UpdateLives(int lives)
	{
		_livesLabel.Text = $"Lives: {lives}";
	}

	public void ShowInfo(string message)
	{
		_infoLabel.Text = message;
		_infoLabel.Visible = true;
	}

	public void HideInfo()
	{
		_infoLabel.Visible = false;
	}
}
