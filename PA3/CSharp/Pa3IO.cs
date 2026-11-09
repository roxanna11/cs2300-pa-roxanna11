using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace GraphicsEngine
{
    /// <summary>
    /// PA3 data-file reader and SVG writer.
    /// PROVIDED CODE -- do not modify this file.
    /// </summary>
    public static class Pa3IO
    {
        public class DataFileException : Exception
        {
            public DataFileException(string message) : base(message) { }
        }

        /// <summary>One step from the data file: its name and its numbers.</summary>
        public class Step
        {
            public string Name;
            public double[] Args;
        }

        /// <summary>Everything read from one data file.</summary>
        public class Scene
        {
            public double[,] Vertices;
            public int[,] Edges;
            public List<Step> Steps = new List<Step>();
        }

        private static readonly Dictionary<string, int> ArgCounts = new Dictionary<string, int>
        {
            { "TRANSLATE", 2 }, { "ROTATE", 1 }, { "ROTATE_ABOUT", 3 },
            { "SCALE", 2 }, { "REFLECT_LINE", 4 }, { "MAP_TRIANGLE", 12 }
        };

        private static readonly char[] Spaces = { ' ', '\t' };

        private static string[] Split(string line)
        {
            return line.Split(Spaces, StringSplitOptions.RemoveEmptyEntries);
        }

        private static double Num(string s)
        {
            if (!double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double v))
            {
                throw new DataFileException("a number could not be read: " + s);
            }
            return v;
        }

        public static Scene ReadDataFile(string path)
        {
            var lines = new List<string>();
            foreach (string raw in File.ReadAllLines(path, Encoding.UTF8))
            {
                int hash = raw.IndexOf('#');
                string line = (hash >= 0 ? raw.Substring(0, hash) : raw).Trim();
                if (line.Length > 0)
                {
                    lines.Add(line);
                }
            }
            int position = 0;

            string NextLine()
            {
                if (position >= lines.Count) throw new DataFileException("file ended early");
                return lines[position++];
            }

            int Header(string expected)
            {
                if (position >= lines.Count) throw new DataFileException("missing " + expected + " line");
                string[] parts = Split(lines[position]);
                if (parts.Length != 2 || !parts[0].Equals(expected, StringComparison.OrdinalIgnoreCase))
                {
                    throw new DataFileException("expected '" + expected + " n', found '" + lines[position] + "'");
                }
                position++;
                return (int)Num(parts[1]);
            }

            var scene = new Scene();
            int n = Header("VERTICES");
            scene.Vertices = new double[n, 2];
            for (int k = 0; k < n; k++)
            {
                string[] parts = Split(NextLine());
                if (parts.Length != 2) throw new DataFileException("each vertex line needs two numbers");
                scene.Vertices[k, 0] = Num(parts[0]);
                scene.Vertices[k, 1] = Num(parts[1]);
            }

            int m = Header("EDGES");
            scene.Edges = new int[m, 2];
            for (int k = 0; k < m; k++)
            {
                string[] parts = Split(NextLine());
                if (parts.Length != 2) throw new DataFileException("each edge line needs two vertex numbers");
                int i = (int)Num(parts[0]);
                int j = (int)Num(parts[1]);
                if (i < 0 || i >= n || j < 0 || j >= n)
                {
                    throw new DataFileException("edge " + i + " " + j + " uses a vertex that does not exist");
                }
                scene.Edges[k, 0] = i;
                scene.Edges[k, 1] = j;
            }

            int s = Header("STEPS");
            for (int k = 0; k < s; k++)
            {
                string[] parts = Split(NextLine());
                string name = parts[0].ToUpperInvariant();
                if (!ArgCounts.ContainsKey(name)) throw new DataFileException("unknown step '" + parts[0] + "'");
                if (parts.Length - 1 != ArgCounts[name])
                {
                    throw new DataFileException(name + " needs " + ArgCounts[name] + " numbers");
                }
                var args = new double[parts.Length - 1];
                for (int a = 0; a < args.Length; a++) args[a] = Num(parts[a + 1]);
                scene.Steps.Add(new Step { Name = name, Args = args });
            }
            return scene;
        }

        private static string F1(double v)
        {
            return v.ToString("0.0", CultureInfo.InvariantCulture);
        }

        /// <summary>Draw the original object (gray, dashed) and the transformed object (blue).</summary>
        public static void WriteSvg(string path, double[,] original, double[,] transformed, int[,] edges)
        {
            double minX = double.PositiveInfinity, maxX = double.NegativeInfinity;
            double minY = double.PositiveInfinity, maxY = double.NegativeInfinity;
            foreach (double[,] shape in new[] { original, transformed })
            {
                for (int k = 0; k < shape.GetLength(0); k++)
                {
                    minX = Math.Min(minX, shape[k, 0]); maxX = Math.Max(maxX, shape[k, 0]);
                    minY = Math.Min(minY, shape[k, 1]); maxY = Math.Max(maxY, shape[k, 1]);
                }
            }
            minX -= 1; maxX += 1; minY -= 1; maxY += 1;
            double span = Math.Max(Math.Max(maxX - minX, maxY - minY), 1e-9);
            double scale = 520.0 / span;
            double mnX = minX, mxY = maxY;
            Func<double, double> sx = x => 20 + (x - mnX) * scale;
            Func<double, double> sy = y => 20 + (mxY - y) * scale;

            var output = new StringBuilder();
            output.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"").Append(F1(40 + (maxX - minX) * scale))
                  .Append("\" height=\"").Append(F1(40 + (maxY - minY) * scale)).Append("\">\n");
            output.Append("<rect width=\"100%\" height=\"100%\" fill=\"white\"/>\n");
            if (span <= 40)
            {
                for (int gx = (int)minX - 1; gx <= (int)maxX + 1; gx++)
                {
                    if (minX <= gx && gx <= maxX)
                    {
                        output.Append("<line x1=\"").Append(F1(sx(gx))).Append("\" y1=\"").Append(F1(sy(minY)))
                              .Append("\" x2=\"").Append(F1(sx(gx))).Append("\" y2=\"").Append(F1(sy(maxY)))
                              .Append("\" stroke=\"#eeeeee\" stroke-width=\"1\"/>\n");
                    }
                }
                for (int gy = (int)minY - 1; gy <= (int)maxY + 1; gy++)
                {
                    if (minY <= gy && gy <= maxY)
                    {
                        output.Append("<line x1=\"").Append(F1(sx(minX))).Append("\" y1=\"").Append(F1(sy(gy)))
                              .Append("\" x2=\"").Append(F1(sx(maxX))).Append("\" y2=\"").Append(F1(sy(gy)))
                              .Append("\" stroke=\"#eeeeee\" stroke-width=\"1\"/>\n");
                    }
                }
            }
            if (minX <= 0 && 0 <= maxX)
            {
                output.Append("<line x1=\"").Append(F1(sx(0))).Append("\" y1=\"").Append(F1(sy(minY)))
                      .Append("\" x2=\"").Append(F1(sx(0))).Append("\" y2=\"").Append(F1(sy(maxY)))
                      .Append("\" stroke=\"#999999\" stroke-width=\"1\"/>\n");
            }
            if (minY <= 0 && 0 <= maxY)
            {
                output.Append("<line x1=\"").Append(F1(sx(minX))).Append("\" y1=\"").Append(F1(sy(0)))
                      .Append("\" x2=\"").Append(F1(sx(maxX))).Append("\" y2=\"").Append(F1(sy(0)))
                      .Append("\" stroke=\"#999999\" stroke-width=\"1\"/>\n");
            }
            string[] colors = { "#888888", "#1f5fbf" };
            string[] dashes = { " stroke-dasharray=\"6,4\"", "" };
            double[][,] shapes = { original, transformed };
            for (int s = 0; s < 2; s++)
            {
                double[,] shape = shapes[s];
                for (int e = 0; e < edges.GetLength(0); e++)
                {
                    int i = edges[e, 0], j = edges[e, 1];
                    output.Append("<line x1=\"").Append(F1(sx(shape[i, 0]))).Append("\" y1=\"").Append(F1(sy(shape[i, 1])))
                          .Append("\" x2=\"").Append(F1(sx(shape[j, 0]))).Append("\" y2=\"").Append(F1(sy(shape[j, 1])))
                          .Append("\" stroke=\"").Append(colors[s]).Append("\" stroke-width=\"3\"").Append(dashes[s]).Append("/>\n");
                }
                for (int k = 0; k < shape.GetLength(0); k++)
                {
                    double x = sx(shape[k, 0]), y = sy(shape[k, 1]);
                    output.Append("<circle cx=\"").Append(F1(x)).Append("\" cy=\"").Append(F1(y))
                          .Append("\" r=\"4\" fill=\"").Append(colors[s]).Append("\"/>\n");
                    output.Append("<text x=\"").Append(F1(x + 6)).Append("\" y=\"").Append(F1(y - 6))
                          .Append("\" font-size=\"13\" font-family=\"sans-serif\" fill=\"").Append(colors[s]).Append("\">")
                          .Append(k).Append("</text>\n");
                }
            }
            output.Append("<text x=\"20\" y=\"16\" font-size=\"13\" font-family=\"sans-serif\" fill=\"#888888\">original (dashed)</text>\n");
            output.Append("<text x=\"160\" y=\"16\" font-size=\"13\" font-family=\"sans-serif\" fill=\"#1f5fbf\">transformed</text>\n");
            output.Append("</svg>\n");
            File.WriteAllText(path, output.ToString(), new UTF8Encoding(false));
        }
    }
}
