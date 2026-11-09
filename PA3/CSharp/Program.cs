using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace GraphicsEngine
{
    /// <summary>
    /// PA3 - Graphics Transformation Engine (driver).
    /// PROVIDED CODE. Run:  dotnet run            (uses pa3_house.txt)
    ///                      dotnet run other.txt  (any data file)
    /// </summary>
    public static class Program
    {
        /// <summary>Avoid printing -0.0000 for values that are zero up to round-off.</summary>
        static double Clean(double v)
        {
            return Math.Abs(v) < 5e-10 ? 0.0 : v;
        }

        static string F(double v, int width, int decimals)
        {
            return Clean(v).ToString("F" + decimals, CultureInfo.InvariantCulture).PadLeft(width);
        }

        static void ShowMatrix(double[,] m)
        {
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("  " + F(m[i, 0], 10, 4) + "  " + F(m[i, 1], 10, 4) + "  " + F(m[i, 2], 10, 4));
            }
        }

        static void ShowPoints(double[,] before, double[,] after)
        {
            for (int k = 0; k < before.GetLength(0); k++)
            {
                Console.WriteLine("  " + k + ": (" + F(before[k, 0], 9, 4) + ", " + F(before[k, 1], 9, 4) + ")  ->  ("
                                  + F(after[k, 0], 9, 4) + ", " + F(after[k, 1], 9, 4) + ")");
            }
        }

        /// <summary>Print a data-file number the way it was typed: 90, 0.5, -1.</summary>
        static string Number(double v)
        {
            if (v == Math.Round(v))
            {
                return ((long)v).ToString(CultureInfo.InvariantCulture);
            }
            string s = v.ToString("F6", CultureInfo.InvariantCulture).TrimEnd('0');
            return s.EndsWith(".") ? s.Substring(0, s.Length - 1) : s;
        }

        static double[] Pt(double[] a, int i)
        {
            return new[] { a[i], a[i + 1] };
        }

        static double[,] StepMatrix(string name, double[] a)
        {
            switch (name)
            {
                case "TRANSLATE": return Transforms.Translate(a[0], a[1]);
                case "ROTATE": return Transforms.Rotate(a[0]);
                case "ROTATE_ABOUT": return Transforms.RotateAbout(a[0], a[1], a[2]);
                case "SCALE": return Transforms.Scale(a[0], a[1]);
                case "REFLECT_LINE": return Transforms.ReflectLine(a[0], a[1], a[2], a[3]);
                case "MAP_TRIANGLE":
                    return Transforms.MapTriangle(Pt(a, 0), Pt(a, 2), Pt(a, 4), Pt(a, 6), Pt(a, 8), Pt(a, 10));
                default: throw new ArgumentException("unknown step " + name);
            }
        }

        public static int Main(string[] args)
        {
            string path = args.Length > 0 ? args[0] : "pa3_house.txt";
            Console.WriteLine("PA3 - Graphics Transformation Engine");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("Data file: " + path);
            Console.WriteLine();

            Pa3IO.Scene scene;
            try
            {
                scene = Pa3IO.ReadDataFile(path);
            }
            catch (Exception e) when (e is IOException || e is Pa3IO.DataFileException || e is UnauthorizedAccessException)
            {
                Console.WriteLine("Error reading data file: " + e.Message);
                return 1;
            }

            var matrices = new List<double[,]>();
            try
            {
                int k = 1;
                foreach (Pa3IO.Step step in scene.Steps)
                {
                    double[,] m = StepMatrix(step.Name, step.Args);
                    matrices.Add(m);
                    string label = "Step " + k + ": " + step.Name;
                    foreach (double a in step.Args)
                    {
                        label += " " + Number(a);
                    }
                    Console.WriteLine(label);
                    ShowMatrix(m);
                    Console.WriteLine();
                    k++;
                }
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Error: " + e.Message);
                Console.WriteLine("Processing of this file stopped.");
                return 1;
            }

            double[,] composite = Transforms.Compose(matrices);
            Console.WriteLine("Composite Matrix M = Mk ... M2 M1");
            Console.WriteLine("---------------------------------");
            ShowMatrix(composite);
            Console.WriteLine();

            double[,] transformed = Transforms.Apply(composite, scene.Vertices);
            Console.WriteLine("Original -> Transformed Vertices");
            Console.WriteLine("--------------------------------");
            ShowPoints(scene.Vertices, transformed);
            Console.WriteLine();

            Console.WriteLine("Order Check (first two steps swapped)");
            Console.WriteLine("-------------------------------------");
            if (matrices.Count < 2)
            {
                Console.WriteLine("  Only one step: there is nothing to swap.");
            }
            else
            {
                var swapped = new List<double[,]>(matrices);
                swapped[0] = matrices[1];
                swapped[1] = matrices[0];
                double[,] swappedComposite = Transforms.Compose(swapped);
                ShowMatrix(swappedComposite);
                double[,] swappedPoints = Transforms.Apply(swappedComposite, scene.Vertices);
                ShowPoints(scene.Vertices, swappedPoints);
                double distance = Transforms.MaxDistance(transformed, swappedPoints);
                Console.WriteLine("  Largest vertex distance between the two orders: " + F(distance, 0, 4));
                Console.WriteLine(distance < Transforms.Tolerance
                    ? "  The two orders give the same object."
                    : "  The two orders give different objects.");
            }
            Console.WriteLine();

            Console.WriteLine("Inverse Matrix");
            Console.WriteLine("--------------");
            try
            {
                double[,] inverse = Transforms.Inverse(composite);
                ShowMatrix(inverse);
                double[,] recovered = Transforms.Apply(inverse, transformed);
                double error = Transforms.MaxDistance(recovered, scene.Vertices);
                Console.WriteLine("  Largest recovery error: " + error.ToString("F12", CultureInfo.InvariantCulture));
                Console.WriteLine(error < Transforms.Tolerance
                    ? "  The original object was recovered (within tolerance)."
                    : "  The original object was NOT recovered.");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("  " + e.Message);
            }
            Console.WriteLine();

            try
            {
                Pa3IO.WriteSvg("pa3_output.svg", scene.Vertices, transformed, scene.Edges);
                Console.WriteLine("Drawing written to pa3_output.svg (open it in a web browser).");
            }
            catch (IOException e)
            {
                Console.WriteLine("Could not write pa3_output.svg: " + e.Message);
            }
            return 0;
        }
    }
}
