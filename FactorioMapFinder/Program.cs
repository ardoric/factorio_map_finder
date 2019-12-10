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

        static int BigIron(Bitmap map, int threshold)
        {
            bool[,] visited = new bool[map.Height, map.Width];
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
                            return patch_size;
                    }
                }
                for (; j < y + l; j++)
                {
                    Color p = map.GetPixel(i, j);
                    if (ores.ContainsKey(p) && ores[p] == "iron")
                    {
                        int patch_size = count_patch(i, j, map, visited);
                        if (patch_size > threshold)
                            return patch_size;
                    }
                }
                for (; i > x - l; i--)
                {
                    Color p = map.GetPixel(i, j);
                    if (ores.ContainsKey(p) && ores[p] == "iron")
                    {
                        int patch_size = count_patch(i, j, map, visited);
                        if (patch_size > threshold)
                            return patch_size;
                    }
                }
                for (; j < y - l; j--)
                {
                    Color p = map.GetPixel(i, j);
                    if (ores.ContainsKey(p) && ores[p] == "iron")
                    {
                        int patch_size = count_patch(i, j, map, visited);
                        if (patch_size > threshold)
                            return patch_size;
                    }
                }


            }

            return 0;

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
            bool[,] visited = new bool[map.Height, map.Width];

            // look at all the visible coals
            for (int j = 100; j < map.Height - 100; j++)
                for (int i = 100; i < map.Width - 100; i++)
                {
                    Color p = map.GetPixel(i, j);
                    if (ores.ContainsKey(p) && ores[p] == "coal")
                    {
                        if (has_close_water(i, j, map, visited, threshold) && count_patch(i,j,map, new bool[map.Width,map.Height]) > 200)
                            return true;
                    }
                }

            return false;
        }

        private static List<Point> all_directions = new List<Point>()
        {
            new Point(1,0),
            new Point(0,1),
            new Point(-1,0),
            new Point(0,-1)
        };

        private static bool has_close_water(int i, int j, Bitmap map, bool[,] visited, int threshold)
        {
            Queue<Point> points = new Queue<Point>();
            points.Enqueue(new Point(i, j));
            Color coal = Color.FromArgb(0, 0, 0);
            
            while (points.Count > 0)
            {
                Point p = points.Dequeue();

                // flat out skip out of bounds points
                if (p.X < 0 || p.X >= map.Width)
                    continue;
                if (p.Y < 0 || p.Y >= map.Height)
                    continue;

                if (visited[p.X, p.Y])
                    continue;

                visited[p.X, p.Y] = true;

                if (map.GetPixel(p.X, p.Y) != coal)
                    continue; // nothing to be done here

                List<Point> directions = new List<Point>();

                // look for water in directions where there's no coal in
                // this will make the heavier part of the algorithm only run on 
                // the edges of patches... hopefully
                foreach (Point d in all_directions)
                {
                    Point np = new Point(p.Y + d.X, p.Y + d.Y);

                    // out of bounds protection
                    if (np.X < 0 || np.X >= map.Width)
                        continue;
                    if (np.Y < 0 || np.Y >= map.Height)
                        continue;

                    if (map.GetPixel(np.X, np.Y) != coal)
                        directions.Add(d);
                }

                // "the heavier part of the algorithm"
                for (int l = 1; directions.Count > 0 && l <= threshold; l++)
                {
                    foreach (Point d in directions)
                    {
                        Point np = new Point(p.X + l*d.X, p.Y + l*d.Y);
                        // out of bounds protection
                        if (np.X < 0 || np.X >= map.Width)
                            continue;
                        if (np.Y < 0 || np.Y >= map.Height)
                            continue;
                        Color c = map.GetPixel(np.X,np.Y);
                        if (ores.ContainsKey(c) && ores[c] == "water")
                            return true;
                        // could stop searching in a direction if coal is found 
                        // as this means the new coal is closer to water in that direction
                    }
                }

                // add all surrounding points to be analyzed also
                for (int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                        points.Enqueue(new Point(p.X + x, p.Y + y));

            }
            return false;
        }

        static void Main(string[] args)
        {
            string base_dir = args[0];
            if (!Directory.Exists(base_dir + "nefrums"))
                Directory.CreateDirectory(base_dir + "nefrums");

            foreach (string f in Directory.GetFiles(base_dir))
            {
                // Console.WriteLine(f);
                Bitmap map = new Bitmap(f);
                if (HasBigIron(map, 6000))
                {
                    if (HasCoalCloseToWater(map, 20))
                    {
                        File.Copy(f, Path.GetDirectoryName(f) + "\\nefrums\\" + Path.GetFileName(f), true);
                        Console.WriteLine(Path.GetDirectoryName(f) + "\\nefrums\\" + Path.GetFileName(f));
                    }
                }
                
                    // map.Save(f.Replace(".png", "") + "_processed.png", ImageFormat.Png);
            }
        }
    }
}
