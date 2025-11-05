using Godot;
using System.Collections.Generic;

public partial class PhysicsChain : Node2D
{
	[Export] public int ChainSegments = 8;            // ≥ 5 per assignment
	[Export] public float SegmentDistance = 30f;
	[Export] public PackedScene SegmentScene;         // assign ChainSegment.tscn
	[Export] public NodePath PlayerPath;              // optional (for interaction)

	private readonly List<RigidBody2D> _segments = new();
	private readonly List<Joint2D> _joints = new();
	private StaticBody2D _anchor;
	private CharacterBody2D _player;

	public override void _Ready()
	{
		_player = GetNodeOrNull<CharacterBody2D>(PlayerPath);
		CreateChain();
	}

	private void CreateChain()
{
	if (SegmentScene == null)
	{
		GD.PushWarning("Assign ChainSegment.tscn to SegmentScene.");
		return;
	}

	// 1) Fixed anchor at this node's position
	_anchor = new StaticBody2D { Name = "Anchor" };
	AddChild(_anchor);
	_anchor.GlobalPosition = GlobalPosition;

	RigidBody2D prev = null;

	for (int i = 0; i < ChainSegments; i++)
	{
		// 2) Make a segment and place it below the anchor
		var seg = SegmentScene.Instantiate<RigidBody2D>();
		AddChild(seg);
		seg.GlobalPosition = _anchor.GlobalPosition + new Vector2(0, (i + 1) * SegmentDistance);

		// Simple collision setup
		seg.CollisionLayer = 1;
		seg.CollisionMask = 1;

		_segments.Add(seg);

		// 3) Create the joint, add it first, then wire it up
		var pin = new PinJoint2D();
		AddChild(pin); // must be in the tree before GetPathTo()

		if (i == 0)
		{
			// Anchor <-> first segment
			pin.NodeA = pin.GetPathTo(_anchor);
			pin.NodeB = pin.GetPathTo(seg);
			// Put the joint at the first segment's position
			pin.GlobalPosition = seg.GlobalPosition;
		}
		else
		{
			// Previous segment <-> this segment
			pin.NodeA = pin.GetPathTo(prev);
			pin.NodeB = pin.GetPathTo(seg);
			// Put the joint midway between the two segments
			pin.GlobalPosition = (prev.GlobalPosition + seg.GlobalPosition) * 0.5f;
		}

		// Optional: reduce weird collisions between linked bodies
		pin.DisableCollision = true;

		_joints.Add(pin);
		prev = seg;
	}
}

	// Kick a given segment (e.g., last link) with an impulse
	public void ApplyForceToSegment(int index, Vector2 impulse)
	{
		if (index < 0 || index >= _segments.Count) return;
		_segments[index].ApplyImpulse(impulse);
	}

	// Example interaction: press Space to yank the last link toward the player
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_accept") && _player != null && _segments.Count > 0)
		{
			var last = _segments[^1];
			Vector2 dir = (_player.GlobalPosition - last.GlobalPosition).Normalized();
			ApplyForceToSegment(_segments.Count - 1, dir * 300f);
		}
	}
}
