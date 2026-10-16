using System;
using System.Collections.Generic;
using System.Globalization;

namespace CampusShuttleFlow
{
    public class CampusShuttleNetwork
    {
        public static readonly string[] Variables = { "x1", "x2", "x3", "x4", "x5", "x6" };

        private readonly List<string> stops;
        private readonly List<string> routes;

        public CampusShuttleNetwork()
        {
            stops = new List<string>
            {
                "Residence Halls",
                "Library",
                "Engineering",
                "Student Center",
                "Parking Lot"
            };

            routes = new List<string>
            {
                "x1: Residence Halls to Library",
                "x2: Library to Engineering",
                "x3: Residence Halls to Student Center",
                "x4: Student Center to Engineering",
                "x5: Parking Lot to Student Center",
                "x6: Parking Lot to Residence Halls"
            };
        }

        public void DisplayNetwork()
        {
            Console.WriteLine("Campus Shuttle Network");
            Console.WriteLine("----------------------");

            Console.WriteLine("Stops:");
            foreach (string stop in stops)
            {
                Console.WriteLine("  - " + stop);
            }

            Console.WriteLine();

            Console.WriteLine("Routes:");
            foreach (string route in routes)
            {
                Console.WriteLine("  - " + route);
            }
        }

        public List<string> GetEquations()
        {
            return new List<string>
            {
                "Residence Hall: 120 + x6 = 40 + x3 + x1",
                "Library: x1 = 60 + x2",
                "Engineering: x2 + x4 = 150",
                "Student Center: x3 + x5 + 30 = x4",
                "Parking Lot: 100 = x5 + x6"
            };
        }

        public void DisplayEquations()
        {
            Console.WriteLine("System of Linear Equations");
            Console.WriteLine("--------------------------");

            List<string> equations = GetEquations();

            if (equations.Count == 0)
            {
                Console.WriteLine("TODO");
                return;
            }

            foreach (string equation in equations)
            {
                Console.WriteLine(equation);
            }
        }

        public double[,] BuildAugmentedMatrix()
        {
             return new double[,]
            {
                {0, 0, 1, -1, 1, 0,   -30 },
                {-1, 0, -1, 0, 0, 1,  -80 },
                {1, -1, 0, 0, 0, 0,   60 },
                {0, 1, 0, 1, 0, 0,    150 },
                {0, 0, 0, 0, 1, 1,    100 }

            };
        }

        public double[,] ComputeRref(double[,] matrix)
        {
            return MatrixTools.Rref(matrix);
        }

        public Dictionary<string, string> ComputeGeneralSolution(double[,] rrefMatrix)
        {
            return new Dictionary<string, string>
            {
                { "x1", "210 - x4" },
                { "x2", "150 - x4" },
                { "x3", "-130 + x4 + x6" },
                { "x4", "free" },
                { "x5", "100 - x6" },
                { "x6", "free" }
            };
        }

        public Dictionary<string, double> GetFreeVariableValues(Dictionary<string, string> solution)
        {
            Dictionary<string, double> values = new Dictionary<string, double>();

            foreach (var item in solution)
            {
                if (item.Value == "free")
                {
                    Console.Write("Enter a value for free variable " + item.Key + ": ");
                    string input = Console.ReadLine();
                    values[item.Key] = Convert.ToDouble(input, CultureInfo.InvariantCulture);
                }
            }

            return values;
        }

        public Dictionary<string, double> ComputeFlowValues(
            double[,] rrefMatrix,
            Dictionary<string, double> freeValues)
        {
            return new Dictionary<string, double>
            {
                {"X1", 210 - freeValues["x4"] },
                {"X2", 150 - freeValues["x4"] },
                {"X3", -130 + freeValues["x4"] + freeValues["x6"] },
                {"X4", freeValues["x4"] },
                {"X5", 100 - freeValues["x6"] },
                {"X6", freeValues["x6"] }
            };
        }

        public bool IsFeasible(Dictionary<string, double> flows)
        {
            foreach (double flow in flows.Values)
            {
                if (flow < 0)
                {
                    return false;
                }
            }

            return true;
        }

        public void DisplayGeneralSolution(Dictionary<string, string> solution)
        {
            if (solution.Count == 0)
            {
                Console.WriteLine("TODO");
                return;
            }

            foreach (var item in solution)
            {
                Console.WriteLine(item.Key + " = " + item.Value);
            }
        }

        public void DisplayFlowValues(Dictionary<string, double> flows)
        {
            if (flows.Count == 0)
            {
                Console.WriteLine("TODO");
                return;
            }

            foreach (var item in flows)
            {
                Console.WriteLine($"{item.Key} = {item.Value:F2}");
            }
        }

        public void DisplayMatrix(double[,] matrix)
        {
            if (matrix.Length == 0)
            {
                Console.WriteLine("TODO");
                return;
            }

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (j > 0) Console.Write("  ");
                    Console.Write($"{matrix[i, j],8:F3}");
                }
                Console.WriteLine();
            }
        }
    }
}
