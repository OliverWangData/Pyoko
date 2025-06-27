#ifndef NOISES_PERLIN_INCLUDE
#define NOISES_PERLIN_INCLUDE

#include "res://_Lib/Shaders/Procedural/Random.gdshaderinc"

// [PERLIN]
// ****************************************************************************************************
// Perlin 2D noise implementation for educational purposes. Based on this video: https://www.youtube.com/watch?v=MJ3bvCkHJtE
// Grid size is 1.

float PrivatePerlinFade(float x)
{
	return ((6.f * x - 15.f) * x + 10.f) * x * x * x;
}

// The sampler is used to determine angle of the corner vectors for the perlin grid.
// NoiseSamples is samples of the following coords:
// Floor(x),		Floor(y)
// Floor(x) + 1,	Floor(y)
// Floor(x),		Floor(y) + 1
// Floor(x) + 1,	Floor(y) + 1
// Where x, y have been applied with the perlin's sampling parameters

// Internal function - Use Perlin2D instead
float PrivatePerlinCalculations(
	float x, float y,															// Coords
	vec4 samples,																// Dependant Noise Samples
	float scale, float xOffset, float yOffset									// Sampling Parameters (Perlin)		Defaults:[1, 0, 0]
	)
{
	// Sampling Parameters
	x = (x / scale) + xOffset;
	y = (y / scale) + yOffset;

	// Smoothing inputs to prevent lerp artifacts when interpolating
	float localX = x - floor(x);
	float localY = y - floor(y);

	vec4 angles = samples * 6.2831853;

	// Dot product of corner vectors and corner-to-sample vectors
	float dotA = cos(angles.x) * localX						+ sin(angles.x) * localY;
	float dotB = cos(angles.y) * (localX - 1.f)				+ sin(angles.y) * localY;
	float dotC = cos(angles.z) * localX						+ sin(angles.z) * (localY - 1.f);
	float dotD = cos(angles.w) * (localX - 1.f)				+ sin(angles.w) * (localY - 1.f);

	// Interpolating
	float valAB = mix(dotA, 	dotB, 		PrivatePerlinFade(localX));
	float valCD = mix(dotC, 	dotD, 		PrivatePerlinFade(localX));
	float value = mix(valAB, 	valCD, 		PrivatePerlinFade(localY)) + 0.5;

	return value;
}

// Parameters
float PerlinRandom(
	float x, float y,															// Coords
	float scale, float xOffset, float yOffset,									// Sampling Parameters (Perlin)		Defaults:[1, 0, 0]
	float seed																	// Random Parameters				Defaults:[0]
	)
{
	vec2 sampling = (vec2(x, y) / scale) + vec2(xOffset, yOffset);

	vec4 samples = vec4(
		Random2D(floor(sampling.x),				floor(sampling.y),			seed),
		Random2D(floor(sampling.x) + 1.f,		floor(sampling.y),			seed),
		Random2D(floor(sampling.x),				floor(sampling.y) + 1.f,	seed),
		Random2D(floor(sampling.x) + 1.f,		floor(sampling.y) + 1.f,	seed)
		);

	return PrivatePerlinCalculations(
		x, y,																	// Coords
		samples,																// Dependant Noise Samples
		scale, xOffset, yOffset													// Sampling Parameters (Perlin)
		);
}

#endif // NOISES_PERLIN_INCLUDE