using Godot;

public partial class ScreenWarp : VisibleOnScreenNotifier2D
{

	private Node2D _entity;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_entity = GetParent() as Node2D;

		if(_entity == null)
		{
			GD.PushError("ScreenWarp must be a child node of Node2D");
			return;
		}
		ScreenExited += OnScreenExited;
	}

	private void OnScreenExited()
	{
		Rect2 screen = GetViewport().GetVisibleRect();
		Vector2 position = _entity.GlobalPosition;

		if(position.X < screen.Position.X)
		{
			position.X = screen.End.X;
		} else if(position.X > screen.End.X)
		{
			position.X = screen.Position.X;
		}

		if(position.Y < screen.Position.Y)
		{
			position.Y = screen.End.Y;
		} else if (position.Y > screen.End.Y)
		{
			position.Y = screen.Position.Y;
		}
		_entity.GlobalPosition = position;
	}
}
