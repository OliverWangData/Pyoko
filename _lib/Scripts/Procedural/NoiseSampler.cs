using System;
using System.Numerics;

namespace SQLib.Procedural
{
    public static class NoiseSampler
    {
        public enum  BaseNoiseType
        {
            Random,
            Perlin,
            CellularValue,
            CellularDistance
        }

        // [Random]
        // ****************************************************************************************************
        // GLSL One Liner pseudo random generator implementation
        // Returns a float in the range of [0, 1]
        // Note: May cause some issues depending on hardware implementation of sin() with really large numbers.
        public static float Random(float x, float y, float seed = 0)
        {
            return SQMath.Fract(
                MathF.Sin(
                    SQMath.Mod(
                        Vector2.Dot(
                            new Vector2(x + seed, y + seed), new Vector2(12.9898f, 78.233f)
                            ) + seed, 6.2831853f
                        ) + seed
                    ) * (43758.5453f + seed)
                );
        }

        // [Perlin]
        // ****************************************************************************************************
        // Perlin 2D noise implementation for educational purposes. Based on this video: https://www.youtube.com/watch?v=MJ3bvCkHJtE
        // Returns a float in the range of [0, 1]

        // Implemented with the following algorithm:
        //
        //		- Set a vector for each corner of the grid
        //		- Get dot product between generated corner vector, and the corner-to-(x,y) vector.
        //		- Linearly interpolate between dot product values to get final value.
        // 
        // Grid size is 1x1, so the positions of the corners of the grid are whole numbers.
        
        public static float Perlin(
            float x, float y,                                                           // Coords
            float scale = 1, float xOffset = 0, float yOffset = 0,                      // Sampling Parameters (Perlin)
            float seed = 0                                                              // Angle sampling Parameters (Random) (Generates angles for corner vectors)
            )
        {
            // Sampling Parameters
            Vector2 coords = GetCoords(x, y, scale, xOffset, yOffset);

            // Generates radian angles for the vectors on the 4 corners. Length of the vectors assumed to be 1.
            Vector4 angles = new Vector4(
                Random(MathF.Floor(coords.X),       MathF.Floor(coords.Y), seed)        * 6.2831853f,
                Random(MathF.Floor(coords.X) + 1f,  MathF.Floor(coords.Y), seed)        * 6.2831853f,
                Random(MathF.Floor(coords.X),       MathF.Floor(coords.Y) + 1f, seed)   * 6.2831853f,
                Random(MathF.Floor(coords.X) + 1f,  MathF.Floor(coords.Y) + 1f, seed)   * 6.2831853f
                );

            float localX = coords.X - MathF.Floor(coords.X);
            float localY = coords.Y - MathF.Floor(coords.Y);

            // Dot product of corner vectors and corner-to-sample vectors
            float dotA = MathF.Cos(angles.X) * localX           + MathF.Sin(angles.X) * localY;
            float dotB = MathF.Cos(angles.Y) * (localX - 1f)    + MathF.Sin(angles.Y) * localY;
            float dotC = MathF.Cos(angles.Z) * localX           + MathF.Sin(angles.Z) * (localY - 1f);
            float dotD = MathF.Cos(angles.W) * (localX - 1f)    + MathF.Sin(angles.W) * (localY - 1f);

            // Interpolation Step
            float valAB = SQMath.Lerp(dotA, dotB, PerlinFade(localX));
            float valCD = SQMath.Lerp(dotC, dotD, PerlinFade(localX));
            float value = SQMath.Lerp(valAB, valCD, PerlinFade(localY)) + 0.5f;
            
            return value;
        }

        // Smooths interpolation factor to prevent sharp artifacts on the grid edges when doing the interpolation step
        private static float PerlinFade(float x)
        {
            return ((6f * x - 15f) * x + 10f) * x * x * x;
        }

        // [Cellular]
        // ****************************************************************************************************
        // Cellular 2D noise implementation for educational purposes.
        // Grid size is 1.

        // Implemented with the following algorithm:
        //		- Divides space into a grid
        //		- Sets random points for the surrounding 3x3 grid cells
        //		- Finds the "center" of the cell by calculating smallest distance between (x, y) and the 9 randomly set points
        //
        //      - (Optional) Do different calculations depending on what type of result you want.
        //          E.g. Use Random(x, y) on the coords of the center to give value to each cell. 
        //
        //			+-------+-------+-------+
        //			|		|		|	.	|
        //			|	.	|	   .|		|
        //			+-------+-------+-------+
        //			|.		|	o	|		|
        //			|		|.		|		|
        //			+-------+-------+-------+
        //			|	   .|  .	|		|
        //			|		|		|	.	|
        //			+-------+-------+-------+
        //
        // Where:	o is the point (x, y)
        //
        //
        // This is the furthest the (x, y) coord can be from any center
        //
        //			+-----------+-----------+
        //			|'		    |		   '|
        //			|			|			|
        //			|			|           |
        //			|			|           |
        //			+-----------o-----------+
        //			|			|           |
        //			|			|           |
        //			|			|           |
        //			|.			|          .|
        //          +-----------+-----------+
        //
        // So the maximum distance is the length of the (xGridSize, yGridSize) vector.
        // This is used to normalize distance between (x, y) and the center.
        //
        // Note that in the current implementation, x and y grid sizes are 1, so the normalization factor is sqrt(2).
        //
        //
        // Distance to edge is calculated by taking the intersections of the line on ab, with the line on c with perpendicular slope to ab. Then take ab/2 - ad
        //
        //			b
        //			|
        //			|
        //			|
        // ---------|---------
        //			|
        //	------- d --- c -----
        //			|
        //			a
        //
        // Where,	a, b are the first and second closest randomly set points
        //			c is the (x, y) position
        //
        // Using linear slope y = m * x + b:
        //
        // abLine:		y = mx + g						where:	m = (b.y - a.y) / (b.x - a.x)
        //			=>	mx - y + g = 0							g = a.y - (m * a.x)
        //
        // cPerpLine:	y = nx + f						where:	n = - 1 / m = - (b.x - a.x) / (b.y - a.y)
        //			=>	nx - y + f = 0							f = c.y - (n * c.x)
        //
        // dPoint:		mx - y + g = nx - y + f = 0		=>		x = (g - f) / (n - m)					This is derived from Cramer's rule. See: https://www.cuemath.com/geometry/intersection-of-two-lines/
        //														y = ((g * n) - (f * m)) / (n - m)
        //
        //
        // Distance to closest edge is normalized by taking half the max distance to closest point.
        // This is because the furthest (max) possible value for distance to edge, is if the (x, y) point is on top of the closest point grid point.

        public struct CellularResult
        {
            public Vector2 Coords;
            public Vector2 ClosestNeighborCoords;
            public float Value;
            public float DistCenter;
            public float DistEdge;
            public float DistClosestNeighbor;
        }
        
        public static CellularResult Cellular(
            float x, float y,                                                           // Inputs
            float scale = 1, float xOffset = 0, float yOffset = 0,                      // Sampling Parameters (Cellular)
            float seed = 0                                                              // Random Parameters
            )
        {
            // Sampling Parameters
            Vector2 coords = GetCoords(x, y, scale, xOffset, yOffset);

            // Generates a cell center point for each surrounding 3x3 grid cell

            float[] pointsX = new float[9];
            float[] pointsY = new float[9];

            int i = 0;
            for (int yGrid = SQMath.IntFloor(coords.Y) - 1; yGrid <= MathF.Floor(coords.Y) + 1; yGrid++) // Grid size is 1, so the (x, y) of the grid are whole numbers. 
            for (int xGrid = SQMath.IntFloor(coords.X) - 1; xGrid <= MathF.Floor(coords.X) + 1; xGrid++)
            {

                pointsX[i] = Random(xGrid, yGrid, seed + xGrid);
                pointsY[i] = Random(yGrid, xGrid, seed + xGrid);
                i++;
            }

            // Loops through each of the surrounding 3x3 cells and finds the closest randomly set center point
            // Note: Can avoid taking the square root of euclidean distance until the end, since we are just comparing inside the loop.

            Vector4 closestCoords = new Vector4(0, 0, 0, 0);    // Track the closest coordinates and distance. The current implementation caches both the closest and second closest for each.
            Vector2 closestDistSquare = new Vector2(18, 18);    // Starts out with maximum squared distance between the point and any center, which is (3 * xGridSize)^2 + (3 * yGridSize)^2 = 9+9 = 18

            i = 0;
            for (int gridY = SQMath.IntFloor(coords.Y) - 1; gridY <= SQMath.IntFloor(coords.Y) + 1; gridY++)
            for (int gridX = SQMath.IntFloor(coords.X) - 1; gridX <= SQMath.IntFloor(coords.X) + 1; gridX++)
                {
                    // Needs point's X position to be dependent on Y and vice versa. Otherwise all the X positions will be the same.
                    // Samples a random point between [0, 1] for x and y, scales that up to [min gridx/y, max gridx/y] x and y values inside the grid
                    float pointX = pointsX[i] + gridX;
                    float pointY = pointsY[i] + gridY;
                    float squareDist = ((pointX - coords.X) * (pointX - coords.X)) + ((pointY - coords.Y) * (pointY - coords.Y));

                    // New second smallest distance
                    if (squareDist < closestDistSquare.Y)
                    {
                        // New smallest distance
                        if (squareDist < closestDistSquare.X)
                        {
                            closestDistSquare.Y = closestDistSquare.X;
                            closestDistSquare.X = squareDist;
                            closestCoords = new Vector4(pointX, pointY, closestCoords.X, closestCoords.Y);
                        }
                        else
                        {
                            closestDistSquare.Y = squareDist;
                            closestCoords = new Vector4(closestCoords.X, closestCoords.Y, pointX, pointY);
                        }
                    }
                    i++;
                }

            CellularResult result;
            result.Coords = new Vector2(closestCoords.X, closestCoords.Y);
            result.ClosestNeighborCoords = new Vector2(closestCoords.Z, closestCoords.W);
            result.Value = Random(closestCoords.X, closestCoords.Y);
            result.DistCenter = MathF.Sqrt(closestDistSquare.X) / 1.41421356237f; // Sqrt(2) normalize distance
            result.DistClosestNeighbor = MathF.Sqrt(closestDistSquare.Y) / 1.41421356237f; // Sqrt(2) normalize distance
            result.DistEdge = 1;
            return result;
        }


        // [Brownian]
        // ****************************************************************************************************
        // Fractal Brownian Motion sampler.

        // For Brownian, adjusting the scale in the coords is the same as adjusting it in the sampler.
        // Adjusting the offset in the coords moves the entire noise, whereas in the sampler the speed changes depending on the octave.
        public static float Brownian(
            BaseNoiseType baseType,
            float x, float y,
            float xOffset = 0, float yOffset = 0,                                       // Sample parameters (Brownian)
            int octaves = 1, float persistance = 0.5f, float lacunarity = 2f,           // Brownian Parameters
            float noiseScale = 1, float noiseXOffset = 0, float noiseYOffset = 0,       // Sample Parameters (Random, Perlin, Cellular, etc)
            float noiseSeed = 0                                                         // Random Parameters
            )
        {
            // Sampling Parameters
            Vector2 coords = new Vector2(x, y) + new Vector2(xOffset, yOffset);
            float value = 0;
            float minVal = 0;
            float maxVal = 0;
            float amplitude = 1;
            float frequency = 1;

            // Brownian octaves
            for (int i = 0; i < octaves; i++)
            {
                float sample;

                // Allows swapping between different methods using branching rather than managed method pointers
                // This means that this method uses only blittable types
                if (baseType == BaseNoiseType.Random) sample = Random(coords.X * frequency, coords.Y * frequency, noiseSeed);
                else if (baseType == BaseNoiseType.Perlin) sample = Perlin(coords.X * frequency, coords.Y * frequency, noiseScale, noiseXOffset, noiseYOffset, noiseSeed);
                else if (baseType == BaseNoiseType.CellularValue) sample = Cellular(coords.X * frequency, coords.Y * frequency, noiseScale, noiseXOffset, noiseYOffset, noiseSeed).Value;
                else if (baseType == BaseNoiseType.CellularDistance) sample = Cellular(coords.X * frequency, coords.Y * frequency, noiseScale, noiseXOffset, noiseYOffset, noiseSeed).DistCenter;
                else sample = 0;

                // The *2-1 Changes range from [0, 1] to [-1, 1].
                value += (sample * 2f - 1f) * amplitude; // Summation of each sample value as they go up octaves

                minVal -= amplitude;
                maxVal += amplitude;

                amplitude *= persistance; // Amplitude decreases as the octaves go up as persistance [0, 1]
                frequency *= lacunarity; // Frequency increases as octaves go up as frequency [1, inf)
            }

            if (maxVal - minVal == 0) return 0;
            return (value - ((maxVal + minVal) / 2f)) / (maxVal - minVal) + 0.5f;
        }
        

        // [Domain Warp]
        // ****************************************************************************************************
        // Domain Distortion / Warping sampler. Based on this site: https://iquilezles.org/articles/warp/

        // The sampler is the base noise to be warp'd
        // NoiseSamples is samples of the following coords:
        // shiftX + 0,		shiftY + 0
        // shiftX + 5.2,	shiftY + 1.3

        public static float Warp(
            BaseNoiseType baseType, BaseNoiseType shiftType,                                // Base type is noise being shifted. Shift type is the noise used for shifting
            float x, float y,
            float xOffset = 0, float yOffset = 0,                                           // Sample Parameters (Warp)
            int layers = 1, float strength = 1,                                             // Warp Parameters

            bool useBrownianOnShift = false,                                                                    // Applies brownian on the noise that is being shifted
           float shiftBrownianXOffset = 0, float shiftBrownianYOffset = 0,
            int shiftOctaves = 1, float shiftPersistance = 0.5f, float shiftLacunarity = 2,
            float shiftNoiseScale = 1, float shiftNoiseXOffset = 0, float shiftNoiseYOffset = 0, float shiftNoiseSeed = 0,

            bool useBrownianOnBase = false,                                                                     // Applies brownian on the noise that is doing the shifting
            float baseBrownianXOffset = 0, float baseBrownianYOffset = 0,
            int baseOctaves = 1, float basePersistance = 0.5f, float baseLacunarity = 2,
            float baseNoiseScale = 1, float baseNoiseXOffset = 0, float baseNoiseYOffset = 0, float baseNoiseSeed = 0
            )
        {
            // Sampling Parameters
            float shiftX = x + xOffset;
            float shiftY = y + yOffset;

            // Domain Warp Layers
            for (int i = 0; i < layers; i++)
            {
                float warpShiftX = WarpBranchingSampler(
                    shiftType,
                    shiftX + 0, shiftY + 0, shiftNoiseScale, shiftNoiseXOffset, shiftNoiseYOffset, shiftNoiseSeed,
                    useBrownianOnShift, shiftBrownianXOffset, shiftBrownianYOffset, shiftOctaves, shiftPersistance, shiftLacunarity
                    );
                float warpShiftY = WarpBranchingSampler(
                    shiftType,
                    shiftX + 5.2f, shiftY + 1.3f, shiftNoiseScale, shiftNoiseXOffset, shiftNoiseYOffset, shiftNoiseSeed,
                    useBrownianOnShift, shiftBrownianXOffset, shiftBrownianYOffset, shiftOctaves, shiftPersistance, shiftLacunarity
                    );

                shiftX += (warpShiftX * 2 - 1) * strength;
                shiftY += (warpShiftY * 2 - 1) * strength;
            }

            return WarpBranchingSampler(
                    baseType,
                    shiftX, shiftY, baseNoiseScale, baseNoiseXOffset, baseNoiseYOffset, baseNoiseSeed,
                    useBrownianOnBase, baseBrownianXOffset, baseBrownianYOffset, baseOctaves, basePersistance, baseLacunarity
                    );
        }

        // Allows swapping between different methods using branching rather than managed method pointers
        // This means that this method uses only blittable types
        private static float WarpBranchingSampler(
            BaseNoiseType baseType,
            float x, float y, float scale = 1, float xOffset = 0, float yOffset = 0, float seed = 0, 
            bool useBrownian = false, float brownianXOffset = 0, float brownianYOffset = 0, int octaves = 1, float persistance = 0.5f, float lacunarity = 2f
            )
        {
            if (useBrownian) return Brownian(baseType, x, y, brownianXOffset, brownianYOffset, octaves, persistance, lacunarity, scale, xOffset, yOffset, seed);
            else if (baseType == BaseNoiseType.Random) return Random(x, y, seed);
            else if (baseType == BaseNoiseType.Perlin) return Perlin(x, y, scale, xOffset, yOffset, seed);
            else if (baseType == BaseNoiseType.CellularValue) return Cellular(x, y, scale, xOffset, yOffset, seed).Value;
            else if (baseType == BaseNoiseType.CellularDistance) return Cellular(x, y, scale, xOffset, yOffset, seed).DistCenter;
            else return 0;
        }

        // [General]
        // ****************************************************************************************************
        private static Vector2 GetCoords(float x, float y, float scale, float xOffset, float yOffset)
        {
            return (new Vector2(x, y) / scale) + new Vector2(xOffset, yOffset);
        }
    }
}