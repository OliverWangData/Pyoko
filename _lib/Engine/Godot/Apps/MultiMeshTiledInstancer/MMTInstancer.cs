using Godot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SQLib.GDEngine.MultiMeshTileInstancer
{
    [Tool]
    public partial class MMTInstancer : TileMap
    {
#if TOOLS
        // [Fields]
        // ****************************************************************************************************
        [Export] public bool Update
        {
            get { return false; }
            set { if (value) Instantiate(); }
        }

        [Export] public InstancingBlueprint[] Blueprints;

        private const int LLOYD_RELAXATION_ITERATIONS = 4;
        private const int CUSTOM_DATA_LAYER = 0;

        // [Properties]
        // ****************************************************************************************************
        public Vector2I TileSize
        {
            get
            {
                return TileSet.TileSize;
            }
        }

        // [Methods]
        // ****************************************************************************************************
        private void Instantiate()
        {
            GD.Print("Updating MultiMeshes.");

            // Loops for each Blueprint
            for (int b = 0; b < Blueprints.Length; b++)
            {
                ConcurrentBag<Vector2I> meshCoords = new();
                Godot.Collections.Array<Vector2I> tileCoords = GetUsedCells(Blueprints[b].TilemapLayer);

                // Multithread loop through all the tiles of the Tilemap
                Parallel.For(0, tileCoords.Count, i =>
                {
                    Vector2I coords = tileCoords[i];
                    TileData tile = GetCellTileData(Blueprints[b].TilemapLayer, coords);

                    // Skips empty tiles
                    if (tile == null)
                    {
                        return;
                    }

                    int density = (int)tile.GetCustomDataByLayerId(CUSTOM_DATA_LAYER);

                    // Determining the position of the Scenes in the tile. 
                    // The algorithm used is Random placement, followed by Lloyd Relaxation to get evenly distributed pseudo random points

                    // Pseudo RNG with two seeds. Should be reproducible unique for every tile coordinate.
                    Random rng = new Random(coords.X ^ coords.Y);
                    Vector2I[] positions = new Vector2I[density];

                    // Generates random starting positions before Lloyd Relaxation
                    for (int j = 0; j < density; j++) positions[j] = new Vector2I(
                            rng.Next(0, TileSize.X - 1),
                            rng.Next(0, TileSize.Y - 1)
                            );

                    /// Lloyd relaxation iterations
                    int iterations = Math.Min(density - 1, LLOYD_RELAXATION_ITERATIONS);
                    while (iterations > 0)
                    {
                        Vector2I[] sums = new Vector2I[density];
                        int[] counts = new int[density];

                        /// For each iteration, we want to:
                        ///     - Find the closest point position for each pixel
                        ///     - Use the average of pixel positions as the new position for each point

                        // Loop through each pixel
                        for (int x = 0; x < TileSize.X; x++)
                        {
                            for (int y = 0; y < TileSize.Y; y++)
                            {
                                int closestIndex = 0;
                                float dist = TileSize.X * TileSize.Y;

                                // Loops through each position until the closest one to the current pixel is found
                                for (int c = 0; c < positions.Length; c++)
                                {
                                    float curDist = (positions[c].X - x) * (positions[c].X - x) + (positions[c].Y - y) * (positions[c].Y - y);
                                    if (curDist < dist)
                                    {
                                        closestIndex = c;
                                        dist = curDist;
                                    }
                                }

                                sums[closestIndex] += new Vector2I(x, y);
                                counts[closestIndex]++;
                            }
                        }

                        // Adjusts the position by using the average of the pixel positions
                        for (int n = 0; n < positions.Length; n++)
                        {
                            if (counts[n] > 0) positions[n] = sums[n] / counts[n];
                        }

                        iterations--;
                    }

                    // Saves the positions of the meshes
                    for (int n = 0; n < positions.Length; n++)
                    {
                        meshCoords.Add((coords * TileSize) + positions[n]);
                    }
                });

                // Sorting the meshCoords x, and then y.
                // Draw order is based on the order of the multimesh's array
                int maxWidth = meshCoords.Count;
                List<Vector2I> positions = meshCoords.OrderBy(coord => coord.Y * maxWidth + coord.X).ToList();

                MultiMeshInstance2D multimesh = (MultiMeshInstance2D)GetNode(Blueprints[b].MultiMesh);
                multimesh.Multimesh.InstanceCount = meshCoords.Count;

                for (int i = 0; i < positions.Count; i++)
                {
                    multimesh.Multimesh.SetInstanceTransform2D(i, new Transform2D(0, positions[i]));
                }
            }
        }
#endif
    }
}