# Assignment-5-IMG-420
Assignment 5 IMG 420 by Munendra Pratap Choudhary
🎮 Assignment 5 – Godot 2D Physics and Systems Demo
📘 Overview

This project demonstrates several core Godot 4.4 (Mono/C#) gameplay systems built as part of Assignment 5 IMG-420.
The project is structured around a Main.tscn scene that integrates particles, physics, laser detection, and player interaction.

🧩 Scene Structure
Main (Node2D)
├── ParticleSystem (Node2D)
│   └── GPUParticles2D – Custom dust shader
├── PhysicsDemo (Node2D)
│   └── PhysicsChain.cs – Procedural rope/chain using PinJoint2D
├── LaserSystem (Node2D)
│   └── LaserDetector.cs – RayCast2D-based security laser
└── Player (CharacterBody2D)
    └── Player.cs – Basic movement and laser interaction

⚙️ Features
Part 1 – Particle System

Uses GPUParticles2D with a custom shader (custom_particle.gdshader).

Animated color gradient and wave distortion for dynamic dust effect.

Part 2 – Physics Chain

Procedurally instantiates multiple RigidBody2D segments.

Connects segments via PinJoint2D for realistic rope physics.

Static anchor at the top; responds to forces and collisions.

Press Space to yank the last chain link toward the player.

Part 3 – Laser System

Implements a RayCast2D to detect collisions and the player.

Draws the beam with Line2D that shortens to the hit point.

Switches color and flashes red when detecting the player.

Optional: emits sound or particles for alarm feedback.

🎮 Controls
Key	Action
⬆️⬇️⬅️➡️
Space	Pull last chain segment (Part 2 test)
🧱 Technical Notes

Minimum Godot version: 4.1 or later (Mono build required for C#).

Collision Layers:

Player → Layer 2

Physics Objects → Layer 1

Laser RayCast can be restricted to Layer 2 for precise detection.

All scripts use C# partial classes for modular organization.

🧠 Development & Credits
👨‍💻 Code Assistance

Portions of the C# implementation and Godot setup guidance were generated and refined with assistance from GPT-5 (OpenAI) for educational and code structuring purposes and used for assistance in documentation.

🎨 Assets (Citation)

Chain Segment Sprite: derived from tutorial resources.

Dust Particle Texture: custom-made for visual polish.

Learning & Inspiration Credit: Start Your Game Creation Journey Today! (Godot beginner tutorial) https://www.youtube.com/watch?v=5V9f3MT86M8

🧾 License

This project was created for academic and learning purposes only.
Assets and code may be reused for personal projects with proper attribution.