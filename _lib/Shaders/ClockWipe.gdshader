shader_type canvas_item;

uniform float Offset : hint_range(0, 1);
uniform float Factor : hint_range(0, 1);
uniform bool Inverse;

void fragment() {
	float factorAngle = Factor * 6.28318530718f;
	float uvAngle = mod((atan(UV.y - 0.5, UV.x - 0.5) + (Offset * 6.28318530718f)) * (Inverse ? -1. : 1.), 6.28318530718f);

	if (0. < uvAngle && uvAngle < factorAngle)
	{
		COLOR = texture(TEXTURE, UV);
	}
	else
	{
		COLOR = vec4(0);
	}
}
