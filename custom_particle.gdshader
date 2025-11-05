shader_type canvas_item;

uniform float wave_intensity = 0.10;
uniform vec4 color_start : source_color = vec4(1.0, 0.5, 0.0, 1.0);
uniform vec4 color_end   : source_color = vec4(1.0, 0.0, 0.5, 1.0);

void fragment() {
    vec2 uv = UV;
    uv.x += sin(uv.y * 10.0 + TIME * 2.0) * wave_intensity;

    float g = clamp(uv.y, 0.0, 1.0);
    vec4 grad = mix(color_start, color_end, g);

    vec4 tex = texture(TEXTURE, UV);          // keep round/soft shape
    COLOR = vec4(grad.rgb, grad.a * tex.a);
}
