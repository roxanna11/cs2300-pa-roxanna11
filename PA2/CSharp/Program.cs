using System;

namespace CampusShuttleFlow
{
    class Program
    {
        static void Main(string[] args)
        {
            CampusShuttleNetwork network = new CampusShuttleNetwork();

            Console.WriteLine("PA2 - Campus Shuttle Flow Analyzer");
            Console.WriteLine("----------------------------------");
            Console.WriteLine();

            network.DisplayNetwork();
            Console.WriteLine();

            network.DisplayEquations();
            Console.WriteLine();

            double[,] matrix = network.BuildAugmentedMatrix();
            Console.WriteLine("Augmented Matrix");
            Console.WriteLine("----------------");
            network.DisplayMatrix(matrix);
            Console.WriteLine();

            double[,] rrefMatrix = network.ComputeRref(matrix);
            Console.WriteLine("Reduced Row-Echelon Form");
            Console.WriteLine("------------------------");
            network.DisplayMatrix(rrefMatrix);
            Console.WriteLine();

            var solution = network.ComputeGeneralSolution(rrefMatrix);
            Console.WriteLine("General Solution");
            Console.WriteLine("----------------");
            network.DisplayGeneralSolution(solution);
            Console.WriteLine();

            var freeValues = network.GetFreeVariableValues(solution);
            var flows = network.ComputeFlowValues(rrefMatrix, freeValues);

            Console.WriteLine();
            Console.WriteLine("Shuttle Flow Scenario");
            Console.WriteLine("---------------------");
            network.DisplayFlowValues(flows);
            Console.WriteLine();

            if (network.IsFeasible(flows))
            {
                Console.WriteLine("This shuttle flow configuration is feasible.");
            }
            else
            {
                Console.WriteLine("This shuttle flow configuration is not feasible.");
            }
        }
    }
}
