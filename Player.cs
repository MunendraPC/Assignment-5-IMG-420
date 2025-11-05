using Godot;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 220f;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 input = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Velocity = input * Speed;
		MoveAndSlide();
	}
}
