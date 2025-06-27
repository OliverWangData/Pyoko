using Godot;
using System;
using System.Linq;

namespace SQLib.GDEngine.ProceduralGenerator
{
    public static class PGExtensions
    {
        public static ImageTexture GetTexture(this Sampler sampler, int width, int height, float startX, float startY, float sampleSize = 1, Gradient colorRamp = null)
        {
            float[,] data = sampler.Sample(width, height, startX, startY, sampleSize);

            // Image is going to use RGBF format, which uses 3x floats per pixel
            const int PIXEL_ELEMENT_COUNT = 3;
            float[] pixels = new float[width * height * PIXEL_ELEMENT_COUNT];

            // Transforms the data array into an RGBF pixels array that Image.CreateFromData can use
            int i = 0;
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                float value = data[x, y];

                if (colorRamp is not null)
                {
                    Color color = colorRamp.Sample(value);
                    pixels[i * PIXEL_ELEMENT_COUNT] = color.R;
                    pixels[i * PIXEL_ELEMENT_COUNT + 1] = color.G;
                    pixels[i * PIXEL_ELEMENT_COUNT + 2] = color.B;
                }
                else
                {
                    pixels[i * PIXEL_ELEMENT_COUNT] = value;
                    pixels[i * PIXEL_ELEMENT_COUNT + 1] = value;
                    pixels[i * PIXEL_ELEMENT_COUNT + 2] = value;
                }
                i++;
            }

            // Creating image using RGBF pixels array
            byte[] pixelBytes = pixels.SelectMany(BitConverter.GetBytes).ToArray(); // Converts float[] to byte[]
            Image image = Image.CreateFromData(width, height, false, Image.Format.Rgbf, pixelBytes);

            // Updates existing ImageTexture asset, or creates a new one if none exist
            return ImageTexture.CreateFromImage(image);
        }
    }
}
