using System;

namespace CampusShuttleFlow
{
    public static class MatrixTools
    {
        public static double[,] CopyMatrix(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double[,] copy = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    copy[i, j] = matrix[i, j];
                }
            }

            return copy;
        }

        public static double[,] Rref(double[,] matrix, double tolerance = 1e-10)
        {
            double[,] mat = CopyMatrix(matrix);

            int rowCount = mat.GetLength(0);
            int colCount = mat.GetLength(1);
            int pivotRow = 0;

            for (int col = 0; col < colCount; col++)
            {
                if (pivotRow >= rowCount)
                {
                    break;
                }

                int pivot = -1;

                for (int row = pivotRow; row < rowCount; row++)
                {
                    if (Math.Abs(mat[row, col]) > tolerance)
                    {
                        pivot = row;
                        break;
                    }
                }

                if (pivot == -1)
                {
                    continue;
                }

                SwapRows(mat, pivotRow, pivot);

                double pivotValue = mat[pivotRow, col];

                for (int c = 0; c < colCount; c++)
                {
                    mat[pivotRow, c] = mat[pivotRow, c] / pivotValue;
                }

                for (int row = 0; row < rowCount; row++)
                {
                    if (row != pivotRow)
                    {
                        double factor = mat[row, col];

                        for (int c = 0; c < colCount; c++)
                        {
                            mat[row, c] = mat[row, c] - factor * mat[pivotRow, c];
                        }
                    }
                }

                pivotRow++;
            }

            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < colCount; col++)
                {
                    if (Math.Abs(mat[row, col]) < tolerance)
                    {
                        mat[row, col] = 0.0;
                    }
                }
            }

            return mat;
        }

        private static void SwapRows(double[,] matrix, int rowA, int rowB)
        {
            if (rowA == rowB)
            {
                return;
            }

            int cols = matrix.GetLength(1);

            for (int col = 0; col < cols; col++)
            {
                double temp = matrix[rowA, col];
                matrix[rowA, col] = matrix[rowB, col];
                matrix[rowB, col] = temp;
            }
        }
    }
}
