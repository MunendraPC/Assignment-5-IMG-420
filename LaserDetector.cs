using Godot;

public partial class LaserDetector : Node2D
{
	[Export] public float LaserLength = 500f;
	[Export] public Color LaserColorNormal = Colors.Green;
	[Export] public Color LaserColorAlert = Colors.Red;
	[Export] public NodePath PlayerPath;

	private RayCast2D _rayCast;
	private Line2D _laser;
	private Node2D _player;
	private bool _alarming;
	private Timer _alarmTimer;

	public override void _Ready()
	{
		SetupRaycast();
		SetupVisuals();

		_player = GetNodeOrNull<Node2D>(PlayerPath);

		_alarmTimer = new Timer { OneShot = false, WaitTime = 0.15 };
		AddChild(_alarmTimer);
		_alarmTimer.Timeout += () =>
		{
			// quick flash while alarming
			if (_alarming)
				_laser.DefaultColor = _laser.DefaultColor == LaserColorAlert ? LaserColorNormal : LaserColorAlert;
		};
	}

	private void SetupRaycast()
	{
		_rayCast = new RayCast2D
		{
			TargetPosition = new Vector2(LaserLength, 0),
			CollideWithAreas = false,
			CollideWithBodies = true,
			Enabled = true
		};

		// (Optional) detect only Player on layer 2:
		// _rayCast.CollisionMask = 1 << 2;

		AddChild(_rayCast);
	}

	private void SetupVisuals()
	{
		_laser = new Line2D
		{
			Width = 3f,
			DefaultColor = LaserColorNormal,
			Antialiased = true
		};
		_laser.Points = new Vector2[] { Vector2.Zero, new Vector2(LaserLength, 0) };
		AddChild(_laser);
	}

	public override void _PhysicsProcess(double delta)
	{
		_rayCast.ForceRaycastUpdate();

		if (_rayCast.IsColliding())
		{
			Vector2 hitPoint = _rayCast.GetCollisionPoint();
			_laser.Points = new Vector2[] { Vector2.Zero, ToLocal(hitPoint) };

			var hit = _rayCast.GetCollider();
			bool hitIsPlayer = hit != null && (hit == _player || hit is CharacterBody2D && ((Node)hit).Name == "Player");

			if (hitIsPlayer)
				TriggerAlarm();
			else if (_alarming)
				ResetAlarm();
		}
		else
		{
			// no collision: draw full length
			_laser.Points = new Vector2[] { Vector2.Zero, new Vector2(LaserLength, 0) };
			if (_alarming)
				ResetAlarm();
		}
	}

	private void TriggerAlarm()
	{
		if (_alarming) return;
		_alarming = true;
		_laser.DefaultColor = LaserColorAlert;
		_alarmTimer.Start();
		GD.Print("ALARM! Player detected!");
		// (Optional) play sound or trigger particles here
	}

	private void ResetAlarm()
	{
		_alarming = false;
		_laser.DefaultColor = LaserColorNormal;
		_alarmTimer.Stop();
	}
}
