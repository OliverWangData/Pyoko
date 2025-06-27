shader_type canvas_item;
render_mode unshaded; // For [Canvas Group Shader]

#include "res://_Lib/Shaders/Procedural/Brownian.gdshaderinc"

// [General - Varyings]
// ****************************************************************************************************
// Sends data from the Vertex shader into the Fragment shader
varying vec2 WorldPosition;

// [Canvas Group Shader]
// ****************************************************************************************************
// CanvasGroup uses a built in material and shader to draw all its children in a single call.
// To add functionality to this shader, these must be included.
// See: https://docs.godotengine.org/en/stable/classes/class_canvasgroup.html
uniform bool IsCanvasGroup = false;
uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

// This is not the current game size. It's the size of the game window as designed by pixels, not after applying scaling / stretching
uniform vec2 canvasGroupBaseScreenResolution = vec2(480., 270.);

// [Pixel Outline 2D]
// ****************************************************************************************************
// Works by shifting the texure by the PixelOutlineFXOffsets to add outlines.
// Shifts the vertices and UVs, so textures can have outlines even if they don't have enough space for them.
//
// Flaws with this method:
//		Doesn't work properly for meshes where scaling the vertex positions from the center does not encapsulate the outline area
//			E.g. A hole inside the mesh, will be shifted to one side rather than closing up to allow for a complete outline inside the hole
//		Doesn't work properly when the outline is much bigger than the object in the texture.
//			E.g. A thin 1 pixel line will not be able to have an outline thicker than 1 pixel, since the offsets use the original texture
//			and so there will be a gap between the outline and the 1 pixel line when offset more than 1 pixel.
uniform bool PixelOutline = false;
uniform float PixelOutlineFXThickness : hint_range(1., 16.) = 1.;
uniform vec4 PixelOutlineFXColor : source_color = vec4(1.);

const vec2 PixelOutlineFXOffsets[4] = {
	vec2(-1, 0), vec2(0, -1), vec2(0, 1), vec2(1, 0)
	};

// [Wind FX 2D]
// ****************************************************************************************************
// Works by shifting the UVs by a certain amount determined by noise. Avoids shifting when UVs are closer to Y = 0 (As to not have the stump shake as well)
uniform bool WindFX = false;
uniform vec2 WindFXDirection;
uniform float WindFXIntensity;
uniform float WindFXStiffness : hint_range(0., 1.) = 0.;
uniform float WindSpeed = 2.;

uniform int WindFXOctaves = 4;
uniform float WindFXPersistence = 1.25;
uniform float WindFXLacunarity = 1.75;

// [Helpers]
// ****************************************************************************************************
// Checks to see if the given UV is outside of the [0, 1] range for X or Y.
bool IsWrapping(vec2 uv)
{
	return (
		uv.x > 1. || uv.y > 1. ||
		uv.x < 0. || uv.y < 0.
		);
}
// ****************************************************************************************************

void vertex()
{
	vec2 vertex = VERTEX;

	WorldPosition = (MODEL_MATRIX * vec4(vertex, 0.f, 1.f)).xy;

	// [Pixel Outline 2D - VERT]
	// ****************************************************************************************************
	if (PixelOutline && !IsCanvasGroup) {
		// Shifts all vertices outwards from center (UV = (0.5, 0.5)) by a factor of the outline thickness
		// See [Pixel Outline 2D] above for details and limitations.
		vertex += (UV * 2.0 - 1.0) * PixelOutlineFXThickness;
	}

	// [Wind FX 2D - VERT]
	// ****************************************************************************************************
	if (WindFX)
	{
		float intensity = max(WindFXIntensity, 0.1);
		vec2 noiseOffset = vec2(TIME * WindSpeed);
		if (WindFXDirection != vec2(0., 0.)) noiseOffset *= normalize(WindFXDirection);

		float value = BrownianPerlinRandom(
			WorldPosition.x, WorldPosition.y,
			1, 0, 0,
			WindFXOctaves, WindFXPersistence, WindFXLacunarity,
			100., noiseOffset.x, noiseOffset.y,
			0
			);

		if (WindFXDirection != vec2(0., 0.)) vertex += value * intensity * normalize(WindFXDirection) * vertex.y * (1. - WindFXStiffness);
		else vertex.x += (value * 2. - 1.) * intensity * vertex.y * (1. - WindFXStiffness);
	}

	VERTEX = vertex;
}


void fragment()
{
	vec4 color = COLOR;
	vec2 uv = UV;

	// [Pixel Outline 2D - FRAG]
	// ****************************************************************************************************
	if (PixelOutline && !IsCanvasGroup)
	{
		// Need to shift the UVs to account for the scale up of the quad vertices in the vertex function.
		// This is size of the texture, extruded to include the displacement of the vertexes due to the outline, normalized
		// E.g. if regular texture was 16x16, TEXTURE_PIXEL_SIZE would be (1/16, 1/16) and the offsets with size 2 outlines would be (1/18,1/18)
		vec2 textureSizeWithOffsets = 1. / (1. / TEXTURE_PIXEL_SIZE + PixelOutlineFXThickness * 2.0);
		vec2 textureScaleFactor = TEXTURE_PIXEL_SIZE / textureSizeWithOffsets;
		// Shifts the UV to include size of the outline and then scales it up by the factor between non-offset and offset texture sizes
		uv = (uv - textureSizeWithOffsets * PixelOutlineFXThickness) * textureScaleFactor;

		// Gets the final outline by overlaying the shape of the texture in all FX offsets to get an alpha shape that includes the outlines
		// Then mixing that with the original texture to set the color as either the original texture, the alpha shape white, or keep it transparent
		vec2 offsetSize = textureSizeWithOffsets * PixelOutlineFXThickness * textureScaleFactor;
		float outlineAlpha = 0.0;

		// Overlaying alpha shape
		for (int i = 0; i < PixelOutlineFXOffsets.length(); i++)
		{
			// Ignores UVs that wrap (Are outside the bounds of [0, 1]).
			vec2 sampleUV = uv + (offsetSize * PixelOutlineFXOffsets[i]);
			if (IsWrapping(sampleUV)) continue;
			outlineAlpha += texture(TEXTURE, sampleUV).a * 255.;
		}
		outlineAlpha = min(outlineAlpha, 1.0);

		// Removes texture wrapping caused by the UV shift from original texture
		vec4 originalColor = texture(TEXTURE, uv);
		if (IsWrapping(uv)) originalColor.a = 0.;
		// Mixing between original texture, and alpha texture set to PixelOutlineFXColor
		color = mix(originalColor, PixelOutlineFXColor, outlineAlpha - originalColor.a);
	}

	// [Pixel Outline 2D - FRAG] + [Canvas Group Shader]
	// ****************************************************************************************************
	else if (PixelOutline && IsCanvasGroup)
	{
		//vec2 offsetSize = SCREEN_PIXEL_SIZE * PixelOutlineFXThickness;
		vec2 offsetSize = PixelOutlineFXThickness / canvasGroupBaseScreenResolution;
		float outlineAlpha = 0.0;

		// Overlaying alpha shape
		for (int i = 0; i < PixelOutlineFXOffsets.length(); i++)
		{
			// Ignores UVs that wrap (Are outside the bounds of [0, 1]).
			vec2 sampleUV = SCREEN_UV + (offsetSize * PixelOutlineFXOffsets[i]);
			outlineAlpha += texture(screen_texture, sampleUV).a * 255.;
		}
		outlineAlpha = min(outlineAlpha, 1.0);

		// Removes texture wrapping caused by the UV shift from original texture
		vec4 originalColor = texture(screen_texture,SCREEN_UV);
		vec4 modulate = COLOR - vec4(1, 1, 1, 0);
		// Mixing between original texture, and alpha texture set to PixelOutlineFXColor
		color = mix(originalColor, PixelOutlineFXColor, outlineAlpha - originalColor.a);
	}
	else if (IsCanvasGroup)
	{
		vec4 canvasGroupColor = texture(screen_texture,SCREEN_UV);
		if (canvasGroupColor.a > 0.001) canvasGroupColor.rgb /= canvasGroupColor.a;
		color *= canvasGroupColor;
	}

	COLOR = color;
}