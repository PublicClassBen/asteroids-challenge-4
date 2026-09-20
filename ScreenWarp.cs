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
	}

	public override void _Process(double delta)
	{
		// ScreenExited never fires for entities that spawn outside and never enter.
		// Hidden notifiers belong to inactive asteroid sizes and must not move the entity.
		if (_entity == null || !IsVisibleInTree())
		{
			return;
		}

		Rect2 screen = GetViewport().GetVisibleRect();
		// The notifier can be offset from the entity's origin and rotates with it.
		Rect2 bounds = GlobalTransform * Rect;
		Vector2 center = bounds.GetCenter();
		Vector2 position = _entity.GlobalPosition;

		// Place the notifier's center on the opposite edge so it becomes visible again.
		if(bounds.End.X <= screen.Position.X)
		{
			position.X += screen.End.X - center.X;
		} else if(bounds.Position.X >= screen.End.X)
		{
			position.X += screen.Position.X - center.X;
		}

		if(bounds.End.Y <= screen.Position.Y)
		{
			position.Y += screen.End.Y - center.Y;
		} else if (bounds.Position.Y >= screen.End.Y)
		{
			position.Y += screen.Position.Y - center.Y;
		}
		_entity.GlobalPosition = position;
	}
}
