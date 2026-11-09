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
            return new double[,] { 
                { 1, 0, tx }, 
                { 0, 1, ty }, 
                { 0, 0, 1 } };
        }

        /// <summary>Counterclockwise rotation about the origin.</summary>
        public static double[,] Rotate(double degrees)
        {
            double t = degrees * (Math.PI / 180.0);
            double c = Math.Cos(t);
            double s = Math.Sin(t);
            return new double[,] { 
                { c, -s, 0 }, 
                { s, c, 0 }, 
                { 0, 0, 1 } };
        }

        public static double[,] Scale(double sx, double sy)
        {
            return new double[,] { 
                { sx, 0, 0 }, 
                { 0, sy, 0 }, 
                { 0, 0, 1 } };
        }

        /// <summary>Rotate about the point (rx, ry): T(r) R T(-r).</summary>
        public static double[,] RotateAbout(double degrees, double rx, double ry)
        {
             return MatMult(Translate(rx, ry), MatMult(Rotate(degrees), Translate(-rx, -ry)));
        }

        /// <summary>Reflect across the line through (qx, qy) with direction (dx, dy): T(q) (2uu^T - I) T(-q).</summary>
        public static double[,] ReflectLine(double qx, double qy, double dx, double dy)
        {
            // TODO: normalize (dx, dy) (throw ArgumentException if it is the zero vector),
            //       form F = 2uu^T - I, and return T(q) F T(-q).
            double magnitude = Math.Sqrt(dx * dx + dy * dy);
            if (magnitude == 0)
            {
                throw new ArgumentException("Direction vector cannot be zero.");
            }
            double ux = dx / magnitude;
            double uy = dy / magnitude;

            // Build the reflection matrix F = 2uu^T - I
            double[,] F = new double[3, 3];

            F[0, 0] = 2 * ux * ux - 1;
            F[0, 1] = 2 * ux * uy;
            F[1, 0] = 2 * uy * ux;
            F[1, 1] = 2 * uy * uy - 1;
            F[2, 2] = 1;

            return MatMult(Translate(qx, qy), MatMult(F, Translate(-qx, -qy)));
        }

        /// <summary>Affine map sending triangle a1,a2,a3 onto b1,b2,b3: A = V' V^-1, translation b1 - A a1.</summary>
        public static double[,] MapTriangle(double[] a1, double[] a2, double[] a3,
                                            double[] b1, double[] b2, double[] b3)
        {
            // Source triangle edge vectors
            double v11 = a2[0] - a1[0];
            double v21 = a2[1] - a1[1];
            double v12 = a3[0] - a1[0];
            double v22 = a3[1] - a1[1];

            // Target triangle edge vectors
            double w11 = b2[0] - b1[0];
            double w21 = b2[1] - b1[1];
            double w12 = b3[0] - b1[0];
            double w22 = b3[1] - b1[1];
            // Determinant of V
            double det = v11 * v22 - v12 * v21;

            if (Math.Abs(det) < Tolerance)
            {
                throw new ArgumentException("Source triangle is degenerate.");
            }

            // Inverse of V
            double inv11 = v22 / det;
            double inv12 = -v12 / det;
            double inv21 = -v21 / det;
            double inv22 = v11 / det;
            // A = V' * V^-1
            double a11 = w11 * inv11 + w12 * inv21;
            double a12 = w11 * inv12 + w12 * inv22;
            double a21 = w21 * inv11 + w22 * inv21;
            double a22 = w21 * inv12 + w22 * inv22;

            // Translation p = b1 - A*a1
            double px = b1[0] - (a11 * a1[0] + a12 * a1[1]);
            double py = b1[1] - (a21 * a1[0] + a22 * a1[1]);

            return new double[,]
            {
                {a11, a12, px },
                {a21, a22, py},
                {0,   0,   1 }
            };
        }

        /// <summary>Matrices are listed in the order applied; return M = Mk ... M2 M1.</summary>
        public static double[,] Compose(List<double[,]> matrices)
        {
            double[,] result = Identity();
            foreach (double[,] m in matrices)
            {
                result = MatMult(m, result);
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
            double a = m[0, 0];
            double b = m[0, 1];
            double c = m[1, 0];
            double d = m[1, 1];

            double px = m[0, 2];
            double py = m[1, 2];

            double det = a * d - b * c;

            if (Math.Abs(det) < Tolerance)
            {
                throw new ArgumentException("Matrix is not invertible.");
            }

            // Inverse of the 2x2 block A
            double a11 = d / det;
            double a12 = -b / det;
            double a21 = -c / det;
            double a22 = a / det;

            // Translation column -A^-1*p
            double tx = -(a11 * px + a12 * py);
            double ty = -(a21 * px + a22 * py);

            return new double[,]
            {
             { a11, a12, tx },
             { a21, a22, ty },
             { 0, 0, 1 }
            }; 
        }

        /// <summary>Largest distance between corresponding points of two lists.</summary>
        public static double MaxDistance(double[,] p, double[,] q)
        {
            double maxDistance = 0.0;

            int n = p.GetLength(0);

            for (int k = 0; k < n; k++)
            {
                double dx = p[k, 0] - q[k, 0];
                double dy = p[k, 1] - q[k, 1];

                double distance = Math.Sqrt(dx * dx + dy * dy);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                }
            }

            return maxDistance;
        }
    }
}
