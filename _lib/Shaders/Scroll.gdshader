shader_type canvas_item;

// Pixels per second
uniform vec2 Direction = vec2(1., 1.);
uniform vec2 Offset = vec2(0, 0);

void fragment() {
	vec2 velocity = normalize(Direction);
	vec2 uv = UV + (TIME * velocity) + Offset;
	COLOR = texture(TEXTURE, uv);
}

