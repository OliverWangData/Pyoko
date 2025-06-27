shader_type canvas_item;

uniform float Strength : hint_range(0, 5);
uniform float Tint : hint_range(0, 1);
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

void fragment() {
	//COLOR = textureLod(SCREEN_TEXTURE, SCREEN_UV, Strength);
	vec4 blur = textureLod(SCREEN_TEXTURE, SCREEN_UV, Strength);
	vec3 tint = vec3(0, 0, 0);
	vec3 colMix = mix(blur.rgb, tint.rgb, Tint);
	COLOR = vec4(colMix, blur.a);

}