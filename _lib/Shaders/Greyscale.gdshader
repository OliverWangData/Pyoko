shader_type canvas_item;

uniform bool Enabled = true;

void fragment() {
    COLOR = texture(TEXTURE, UV);
	
	if (Enabled)
	{
	    float avg = (COLOR.r + COLOR.g + COLOR.b) / 3.0;
	    COLOR.rgb = vec3(avg);
	}
}