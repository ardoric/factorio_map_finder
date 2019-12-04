using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioMapFinder
{
    class Program
    {
        static Dictionary<Color, string> ores = new Dictionary<Color, string>()
        {
            { Color.FromArgb(104,132,146), "iron" },
            { Color.FromArgb(203,97,53), "copper" },
            { Color.FromArgb(0,0,0), "coal" },
            { Color.FromArgb(174,154,107), "stone" },
            { Color.FromArgb(51,83,95), "water" },
            { Color.FromArgb(38,64,73), "water" },
        };

        static int count_patch(int i, int j, Bitmap map, bool[,] visited)
        {
            Queue<Point> points = new Queue<Point>();
            // bool[,] visited = new bool[map.Width, map.Height];
            Color patch_color = map.GetPixel(i, j);

            int res = 0;

            points.Enqueue(new Point(i, j));
            while (points.Count > 0)
            {
                Point p = points.Dequeue();

                if (visited[p.X, p.Y])
                    continue;
                visited[p.X, p.Y] = true;

                if (map.GetPixel(p.X, p.Y) == patch_color)
                {
                    res = res + 1;

                    for (int x = -1; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                            points.Enqueue(new Point(p.X + x, p.Y + y));

                    // map.SetPixel(p.X, p.Y, Color.HotPink);
                }


            }


            // Console.WriteLine(string.Format("{0}", res));
            return res;
        }

        static bool HasBigIron(Bitmap map, int threshold)
        {
            bool[,] visited = new bool[map.Height,map.Width];
            int x = map.Width / 2;
            int y = map.Height / 2;

            for (int l = 0; l < 100; l++)
            // look for iron in a 200x200 square from beginning
            {
                int j = y - l;
                int i = x - l;

                for (; i < x + l; i++)
                {
                    Color p = map.GetPixel(i, j);
                    if (ores.ContainsKey(p) && ores[p] == "iron")
                    {
                        int patch_size = count_patch(i, j, map, visited);
                        if (patch_size > threshold)
                            return true;
                    }
                }
                for (; j < y+l; j++)
                {
                    Color p = map.GetPixel(i, j);
                    if (ores.ContainsKey(p) && ores[p] == "iron")
                    {
                        int patch_size = count_patch(i, j, map, visited);
                        if (patch_size > threshold)
                            return true;
                    }
                }
                for (; i > x - l; i--)
                {
                    Color p = map.GetPixel(i, j);
                    if (ores.ContainsKey(p) && ores[p] == "iron")
                    {
                        int patch_size = count_patch(i, j, map, visited);
                        if (patch_size > threshold)
                            return true;
                    }
                }
                for (; j < y - l; j--)
                {
                    Color p = map.GetPixel(i, j);
                    if (ores.ContainsKey(p) && ores[p] == "iron")
                    {
                        int patch_size = count_patch(i, j, map, visited);
                        if (patch_size > threshold)
                            return true;
                    }
                }


            }

            return false;
        }
        static bool HasCoalCloseToWater(Bitmap map, int threshold)
        {
            return false;
        }

        static void Main(string[] args)
        {
            string base_dir = args[0];
            if (!Directory.Exists(base_dir + "bigiron"))
                Directory.CreateDirectory(base_dir + "bigiron");

            foreach (string f in Directory.GetFiles(base_dir))
            {
                // Console.WriteLine(f);
                Bitmap map = new Bitmap(f);
                if (HasBigIron(map, 3000))
                {
                    File.Copy(f, Path.GetDirectoryName(f) + "\\bigiron\\" + Path.GetFileName(f), true);
                    Console.WriteLine(Path.GetDirectoryName(f) + "\\bigiron\\" + Path.GetFileName(f));
                }
                
                    // map.Save(f.Replace(".png", "") + "_processed.png", ImageFormat.Png);
            }
        }
    }
}
