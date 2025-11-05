using Godot;

public partial class ParticleController : GpuParticles2D
{
	private ShaderMaterial _shaderMaterial;
	private float _time;

	public override void _Ready()
	{
		// --- VISUAL MATERIAL (canvas_item shader) ---
		_shaderMaterial = new ShaderMaterial();
		_shaderMaterial.Shader = GD.Load<Shader>("res://custom_particle.gdshader");
		Material = _shaderMaterial;

		// --- PARTICLE BEHAVIOR (process material) ---
		var pm = new ParticleProcessMaterial
		{
			// 2D emission/motion parameters live on the process material:
			// emit upward
			Spread = 25.0f,
			InitialVelocityMin = 60f,
			InitialVelocityMax = 120f,
			// gentle downward pull
			ScaleMin = 0.6f,
			ScaleMax = 1.2f
		};
		ProcessMaterial = pm;

		// --- These are properties of the GpuParticles2D node (NOT the process material) ---
		Lifetime = 2.0f;
		Explosiveness = 0.0f;
		Preprocess = 1.0f;   // start pre-warmed
		Randomness = 0.4f;

		Amount = 250;
		Emitting = true;
	}

	public override void _Process(double delta)
	{
		_time += (float)delta;

		// Animate wave intensity over time
		float wave = 0.08f + Mathf.Sin(_time * 1.8f) * 0.04f;
		_shaderMaterial.SetShaderParameter("wave_intensity", wave);

		// Slowly drift the gradient colors
		var a = new Color(1f, 0.5f, 0f, 1f); // orange
		var b = new Color(1f, 0f, 0.5f, 1f); // magenta
		float t = (Mathf.Sin(_time * 0.6f) + 1f) * 0.5f;
		var start = a.Lerp(b, 0.25f * t);
		var end   = b.Lerp(a, 0.25f * (1f - t));

		_shaderMaterial.SetShaderParameter("color_start", start);
		_shaderMaterial.SetShaderParameter("color_end", end);
	}
}
