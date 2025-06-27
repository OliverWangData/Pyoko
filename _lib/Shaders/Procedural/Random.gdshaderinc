#ifndef NOISES_RANDOM_INCLUDE
#define NOISES_RANDOM_INCLUDE

// [RANDOM]
// ****************************************************************************************************
// Classic one-liner random 2D noise.
//
// Returns a float in the range of [0, 1]
//
// Note: May cause some issues depending on hardware implementation of sin() with really large numbers.

float Random2D(
	float x, float y,															// Coords
	float seed																	// Random Parameters
	)
{
	return fract(
		sin(
			mod(
				dot(
					vec2(x + seed, y + seed), vec2(12.9898,78.233)
					) + seed, 6.2831853
				) + seed
			) * (43758.5453 + seed)
		);
}

#endif // NOISES_RANDOM_INCLUDE