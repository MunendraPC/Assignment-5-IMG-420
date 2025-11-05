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
			GD.PushError("PhysicsChain: SegmentScene is NULL. Drag res://ChainSegment.tscn into 'Segment Scene' in the Inspector.");
			return;
		}

		// 1) Fixed anchor at this node's position
		_anchor = new StaticBody2D { Name = "Anchor" };
		AddChild(_anchor);
		_anchor.GlobalPosition = GlobalPosition;

		GD.Print($"[Rope] Anchor at {_anchor.GlobalPosition}");

		RigidBody2D prev = null;

		for (int i = 0; i < ChainSegments; i++)
		{
			// 2) Make a segment and place it below the anchor
			var seg = SegmentScene.Instantiate<RigidBody2D>();
			if (seg == null)
			{
				GD.PushError("PhysicsChain: SegmentScene root is NOT RigidBody2D. Fix ChainSegment.tscn root.");
				return;
			}

			AddChild(seg);
			seg.Name = $"ChainSegment_{i}";
			seg.GlobalPosition = _anchor.GlobalPosition + new Vector2(0, (i + 1) * SegmentDistance);
			seg.CollisionLayer = 1;
			seg.CollisionMask  = 1;

			_segments.Add(seg);
			GD.Print($"[Rope] Spawned segment #{i} at {seg.GlobalPosition}");

			// 3) Create the joint, add it first, then wire it up
			var pin = new PinJoint2D { Name = $"Joint_{i}", DisableCollision = true };
			AddChild(pin); // must be in the tree before GetPathTo()

			if (i == 0)
			{
				// Anchor <-> first segment
				pin.NodeA = pin.GetPathTo(_anchor);
				pin.NodeB = pin.GetPathTo(seg);
				pin.GlobalPosition = seg.GlobalPosition; // joint at the first link
			}
			else
			{
				// Previous segment <-> this segment
				pin.NodeA = pin.GetPathTo(prev);
				pin.NodeB = pin.GetPathTo(seg);
				pin.GlobalPosition = (prev.GlobalPosition + seg.GlobalPosition) * 0.5f; // midpoint
			}

			_joints.Add(pin);
			prev = seg;

			GD.Print($"[Rope] Joint_{i}: {pin.NodeA} <-> {pin.NodeB} at {pin.GlobalPosition}");
		}
	}

	// Kick a given segment (e.g., last link) with an impulse
	public void ApplyForceToSegment(int index, Vector2 impulse)
	{
		if (index < 0 || index >= _segments.Count) return;
		_segments[index].ApplyImpulse(impulse);
	}

	// Example interaction: press Space to yank the last link toward the player
	public override void _UnhandledInput(InputEvent e)
	{
		if (e.IsActionPressed("ui_accept") && _player != null && _segments.Count > 0)
		{
			var last = _segments[^1];
			Vector2 dir = (_player.GlobalPosition - last.GlobalPosition).Normalized();
			last.ApplyImpulse(dir * 300f);
			GD.Print("[Rope] Yanked last link toward player.");
		}
	}
}
