using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.services
{
    public static class TwoDIntArrayToCodeLines
    {
        public static List<string> CSharp(int[][] grid)
        {
            List<string> rows = new List<string>();
            rows.Add($"int[][] grid = new int[{grid.Length}][];");
            for (int row = 0; row < grid.Length; row++)
            {
                string values = string.Join(",", grid[row]);
                rows.Add("grid[" + row + "] = new int[] { " + values + "};");
            }
            return rows;
        }

        public static List<string> Java(int[][] grid)
        {
            List<string> rows = new List<string>();
            rows.Add("int[][] grid = {");
            for (int row = 0; row < grid.Length; row++)
            {
                string values = string.Join(",", grid[row]);
                if (row == grid.Length-1)
                {
                    rows.Add("\t{ " + values + " }");
                } else
                {
                    rows.Add("\t{ " + values + " },");
                }
            }
            rows.Add("};");
            return rows;
        }

    }




    public static class LeetcodeServices
    {

        /// <summary>
        /// Convert black (really any color) and white images to 2D int array where white pixels are 0 and black (or other) pixels are 1
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static int[][] ConvertImageTo2DArray(Image<Rgba32> image)
        {
            int[][] grid = new int[image.Height][];
            image.ProcessPixelRows(accessor =>
            {
                // Color is pixel-agnostic, but it's implicitly convertible to the Rgba32 pixel type
                Rgba32 transparent = Color.Transparent;
                for (int row = 0; row < accessor.Height; row++)
                {
                    Span<Rgba32> pixelRow = accessor.GetRowSpan(row);
                    grid[row] = new int[image.Width];
                    // pixelRow.Length has the same value as accessor.Width,
                    // but using pixelRow.Length allows the JIT to optimize away bounds checks:
                    for (int col = 0; col < pixelRow.Length; col++)
                    {
                        // Get a reference to the pixel at position x
                        ref Rgba32 pixel = ref pixelRow[col];
                        if (pixel.G == 0 || pixel.R == 0 || pixel.B == 0)
                        {
                            grid[row][col] = 1;
                        }
                    }
                }
            });
            return grid;
        }
    }
}
