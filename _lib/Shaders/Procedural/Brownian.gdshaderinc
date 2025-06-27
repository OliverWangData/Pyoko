#ifndef NOISES_BROWNIAN_INCLUDE
#define NOISES_BROWNIAN_INCLUDE

#include "res://_Lib/Shaders/Macros.gdshaderinc"
#include "res://_Lib/Shaders/Procedural/Perlin.gdshaderinc"

// [BROWNIAN]
// ****************************************************************************************************
// Fractal Brownian Motion sampler.

// The sampler is the base noise to be brownian'd
// NoiseSamples is samples of the following coords:
// x * frequency,	y * frequency
void PrivateBrownianOctaveTask(float sample, inout float value, inout float minVal, inout float maxVal, inout float amplitude, inout float frequency, float persistance, float lacunarity)
{
	// The *2-1 Changes range from [0, 1] to [-1, 1].
	value += (sample * 2. - 1.) * amplitude; // Summation of each sample value as they go up octaves

	minVal -= amplitude;
	maxVal += amplitude;

	amplitude *= persistance; // Amplitude decreases as the octaves go up as persistance [0, 1]
	frequency *= lacunarity; // Frequency increases as octaves go up as frequency [1, inf)
}

float PrivateBrownianCalculations(float value, float minVal, float maxVal)
{
	if (maxVal - minVal == 0.) return 0.;
	return (value - ((maxVal + minVal) / 2.)) / (maxVal - minVal) + 0.5;
}


// [PERLIN]
// ****************************************************************************************************
float BrownianPerlinRandom(
	float x, float y,
	float scale, float xOffset, float yOffset,									// Sample parameters (Brownian) 	Defaults:[1, 0, 0]
	int octaves, float persistance, float lacunarity,							// Brownian Parameters				Defaults:[1, 0.5, 2]
	float perlinScale, float perlinXOffset, float perlinYOffset,				// Sample Parameters (Perlin)		Defaults:[1, 0, 0]
	float seed																	// Random Parameters				Defaults:[0]
	)
{
	// Sampling Parameters
	vec2 sampling = (vec2(x, y) / scale) + vec2(xOffset, yOffset);
	float sample = 0.;
	float value = 0.;
	float minVal = 0.;
	float maxVal = 0.;
	float amplitude = 1.;
	float frequency = 1.;

	// The preprocessor really doesn't like the code block in the RepeatTask macro argument being on new lines, so we have to deal with an ugly one liner
	RepeatTask({ PrivateBrownianOctaveTask(PerlinRandom(sampling.x * frequency, sampling.y * frequency, perlinScale, perlinXOffset, perlinYOffset, seed), value, minVal, maxVal, amplitude, frequency, persistance, lacunarity); }, octaves);

	return PrivateBrownianCalculations(value, minVal, maxVal);
}

#endif // NOISES_BROWNIAN_INCLUDE