using System;
using System.Collections.Generic;

namespace GraphicsEngine
{
    /// <summary>
    /// PA3 transformation engine: 3x3 homogeneous matrices for 2D points.
    /// Every matrix is a double[3,3]. Every list of points is a double[n,2],
    /// where row k holds (x, y) for vertex k.
    /// </summary>
    public static class Transforms
    {
        public const double Tolerance = 1e-9;

        public static double[,] Identity()
        {
            return new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
        }

        /// <summary>Return the 3x3 product a*b.</summary>
        public static double[,] MatMult(double[,] a, double[,] b)
        {
            double[,] c = new double[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    double sum = 0.0;
                    for (int k = 0; k < 3; k++)
                    {
                        sum += a[i, k] * b[k, j];
                    }
                    c[i, j] = sum;
                }
            }
            return c;
        }

        public static double[,] Translate(double tx, double ty)
        {
            return new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { tx, ty, 1 } };
        }

        /// <summary>Counterclockwise rotation about the origin.</summary>
        public static double[,] Rotate(double degrees)
        {
            double t = degrees * (Math.PI / 180.0);
            double c = Math.Cos(t);
            double s = Math.Sin(t);
            return new double[,] { { c, s, 0 }, { -s, c, 0 }, { 0, 0, 1 } };
        }

        public static double[,] Scale(double sx, double sy)
        {
            return new double[,] { { sx, 0, 0 }, { 0, sy, 0 }, { 0, 0, 1 } };
        }

        /// <summary>Rotate about the point (rx, ry): T(r) R T(-r).</summary>
        public static double[,] RotateAbout(double degrees, double rx, double ry)
        {
            // TODO: build T(r) R T(-r) from Translate(), Rotate(), and MatMult().
            return Identity();
        }

        /// <summary>Reflect across the line through (qx, qy) with direction (dx, dy): T(q) (2uu^T - I) T(-q).</summary>
        public static double[,] ReflectLine(double qx, double qy, double dx, double dy)
        {
            // TODO: normalize (dx, dy) (throw ArgumentException if it is the zero vector),
            //       form F = 2uu^T - I, and return T(q) F T(-q).
            return Identity();
        }

        /// <summary>Affine map sending triangle a1,a2,a3 onto b1,b2,b3: A = V' V^-1, translation b1 - A a1.</summary>
        public static double[,] MapTriangle(double[] a1, double[] a2, double[] a3,
                                            double[] b1, double[] b2, double[] b3)
        {
            // TODO: form V and V', compute V^-1 (throw ArgumentException if det V is 0),
            //       A = V' V^-1, and the translation column b1 - A a1. Return the 3x3 matrix.
            return Identity();
        }

        /// <summary>Matrices are listed in the order applied; return M = Mk ... M2 M1.</summary>
        public static double[,] Compose(List<double[,]> matrices)
        {
            double[,] result = Identity();
            foreach (double[,] m in matrices)
            {
                result = MatMult(result, m);
            }
            return result;
        }

        /// <summary>Apply m to every point.</summary>
        public static double[,] Apply(double[,] m, double[,] points)
        {
            int n = points.GetLength(0);
            double[,] output = new double[n, 2];
            for (int k = 0; k < n; k++)
            {
                double x = points[k, 0];
                double y = points[k, 1];
                output[k, 0] = m[0, 0] * x + m[0, 1] * y + m[0, 2];
                output[k, 1] = m[1, 0] * x + m[1, 1] * y + m[1, 2];
            }
            return output;
        }

        /// <summary>Inverse by the block formula [[A^-1, -A^-1 p], [0, 0, 1]].</summary>
        public static double[,] Inverse(double[,] m)
        {
            // TODO: use the block formula with a 2x2 inverse (throw ArgumentException if det A is 0).
            return Identity();
        }

        /// <summary>Largest distance between corresponding points of two lists.</summary>
        public static double MaxDistance(double[,] p, double[,] q)
        {
            // TODO: return the largest distance between p[k] and q[k].
            return 0.0;
        }
    }
}
