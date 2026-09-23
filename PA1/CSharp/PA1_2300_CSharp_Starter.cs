/*
 * CS 2300 Computational Linear Algebra
 * Programming Assignment #1 - Warehouse Navigation Geometry Engine
 * C# Starter Code
 *
 * IMPORTANT - READ BEFORE YOU BEGIN
 * ---------------------------------
 * This starter code contains BOTH:
 *
 *   (1) incomplete methods, which are marked with TODO, and
 *   (2) intentionally incorrect implementations, which are NOT marked.
 *
 * There are exactly FIVE intentionally incorrect implementations.
 *
 * You are responsible for identifying and correcting all of these issues
 * using the mathematical definitions covered in Lectures 01-02.
 *
 * Do not assume a method is correct simply because it runs without an error.
 * Check the numbers the program prints against the definitions from lecture.
 * All five errors are visible in this program's output: if a printed value
 * does not match what you calculate by hand, you have found one.
 *
 * Run with:      dotnet run [input_file]
 * Default input: test_input.txt
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

// =========================
// Vector3D Class
// =========================
public class Vector3D
{
    public double X;
    public double Y;
    public double Z;

    public Vector3D(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    // Adds two vectors
    public Vector3D Add(Vector3D other)
    {
        return new Vector3D(X + other.X,
                            Y + other.Y,
                            Z + other.Z);
    }

    // Subtracts two vectors
    public Vector3D Subtract(Vector3D other)
    {
        // TODO: implement vector subtraction
        //Console.WriteLine("  TODO: Subtract() not implemented");
        //subtract each vector unit for the other respectivly 
        return new Vector3D(X - other.X , Y - other.Y , Z - other.Z);
    }

    // Scalar multiplication
    public Vector3D ScalarMultiply(double scalar)
    {
        return new Vector3D(X * scalar,
                            Y * scalar,
                            Z * scalar);
        //used the Y scalar 
    }

    // Computes vector magnitude
    public double Magnitude()
    {   //was giving just the mag sqaured 
        return Math.Sqrt(X * X + Y * Y + Z * Z);
    }

    // Normalizes a vector (returns a unit vector in the same direction)
    public Vector3D Normalize()
    {
        double mag = Magnitude();

        if (mag == 0)
        {
            return null;
        }
        //divided by 10 no mag
        return new Vector3D(X / mag,
                            Y / mag,
                            Z / mag);
    }

    // Computes the dot product of two vectors
    public double Dot(Vector3D other)
    {   //typo - X * other.Y 
        return X * other.X +
               Y * other.Y +
               Z * other.Z;
    }

    // Computes the cross product of two vectors
    public Vector3D Cross(Vector3D other)
    {
        // TODO: implement the cross product
        //Console.WriteLine("  TODO: Cross() not implemented");

        //take each term multiply and subtract by the others - 
        double x = Y * other.Z - Z * other.Y;
        double y = Z * other.X - X * other.Z;
        double z = X * other.Y - Y * other.X;

        return new Vector3D(x, y, z);
    }

    // Calculates the distance between two points
    public double DistanceTo(Vector3D other)
    {
        // TODO: implement the distance calculation
       // Console.WriteLine("  TODO: DistanceTo() not implemented");
        //sqaure difference of each term - sum them - take square root 
        double dx = other.X - X;
        double dy = other.Y - Y;
        double dz = other.Z - Z;
        double solution = Math.Sqrt(dx *  dx + dy * dy + dz * dz);

        return solution;
    }

    public override string ToString()
    {
        return string.Format(CultureInfo.InvariantCulture,
                             "<{0:F3}, {1:F3}, {2:F3}>", X, Y, Z);
    }
}

// =========================
// Line3D Class
// =========================
public class Line3D
{
    public Vector3D Point;
    public Vector3D Direction;

    public Line3D(Vector3D point, Vector3D direction)
    {
        Point = point;
        Direction = direction;
    }

    // Returns the point on this line at parameter t
    public Vector3D Evaluate(double t)
    {
        // TODO: implement parametric line evaluation --- p(t) = p  + t D
       // Console.WriteLine("  TODO: Evaluate() not implemented");
        //get points + t * direction 
        double x = Point.X + (t * Direction.X);
        double y = Point.Y + (t * Direction.Y);
        double z = Point.Z + (t * Direction.Z);

       
        return new Vector3D( x, y, z );
    }
}

// =========================
// Plane3D Class
// =========================
public class Plane3D
{
    public double A;
    public double B;
    public double C;
    public double D;

    public Plane3D(double a, double b, double c, double d)
    {
        A = a;
        B = b;
        C = c;
        D = d;
    }

    // Builds a plane that passes through three given points.
    // Returns null if the plane cannot be constructed.
    public static Plane3D FromThreePoints(Vector3D p1, Vector3D p2, Vector3D p3)
    {
        // TODO: build the plane from three points.
        // Hint: the normal vector is perpendicular to two vectors that lie
        // in the plane, and D can then be recovered from any point on it.
        Console.WriteLine("  TODO: FromThreePoints() not implemented");

        return null;
    }

    // Evaluates Ax + By + Cz - D at the given point.
    // The sign of this value tells you which side of the plane the point is on.
    public double PlaneValue(Vector3D point)
    {
        return A * point.X +
               B * point.Y +
               C * point.Z - D;
    }

    // Returns true if travelling from oldPoint to newPoint crosses this wall
    public bool CrossedWall(Vector3D oldPoint, Vector3D newPoint)
    {
        double oldSide = PlaneValue(oldPoint);
        double newSide = PlaneValue(newPoint);

        return oldSide == newSide;
    }

    public override string ToString()
    {
        return string.Format(CultureInfo.InvariantCulture,
                             "normal <{0:F3}, {1:F3}, {2:F3}>, d = {3:F3}",
                             A, B, C, D);
    }
}

// =========================
// Robot Class
// =========================
public class Robot
{
    public Vector3D Position;
    public Vector3D StartPosition;

    public Robot(Vector3D position)
    {
        Position = position;
        StartPosition = position;
    }

    // Attempts to move the robot. Returns true if the move was accepted.
    public bool Move(Vector3D movementVector, List<Plane3D> walls)
    {
        Vector3D proposedPosition = Position.Add(movementVector);

        for (int index = 0; index < walls.Count; index++)
        {
            Plane3D wall = walls[index];

            double oldValue = wall.PlaneValue(Position);
            double newValue = wall.PlaneValue(proposedPosition);

            Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "  Wall {0} check: f(old) = {1:F3}, f(new) = {2:F3}",
                index, oldValue, newValue));

            if (wall.CrossedWall(Position, proposedPosition))
            {
                Console.WriteLine($"  Movement rejected by wall {index}.");
                Console.WriteLine($"  Robot remains at {Position}");

                return false;
            }
        }

        Position = proposedPosition;

        Console.WriteLine("  Move accepted.");
        Console.WriteLine($"  Robot new position: {Position}");

        return true;
    }
}

// =========================
// Main Program / File Processing
// =========================
public class PA1_2300_CSharp_Starter
{
    // Distance within which the robot is considered to have retrieved an item.
    public const double PICKUP_RADIUS = 1.0;

    private static double ParseNumber(string text)
    {
        return double.Parse(text, CultureInfo.InvariantCulture);
    }

    public static void ProcessFile(string filename)
    {
        Robot robot = null;
        Vector3D item = null;
        List<Plane3D> walls = new List<Plane3D>();
        Line3D route = null;

        string[] lines = File.ReadAllLines(filename);

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();

            // Skip blank lines and comments
            if (line == "" || line.StartsWith("#"))
            {
                continue;
            }

            string[] parts = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            string command = parts[0];

            // -------------------------
            // ROBOT x y z
            // -------------------------
            if (command == "ROBOT")
            {
                robot = new Robot(
                    new Vector3D(ParseNumber(parts[1]),
                                 ParseNumber(parts[2]),
                                 ParseNumber(parts[3]))
                );

                Console.WriteLine($"Robot Start Position: {robot.Position}\n");
            }

            // -------------------------
            // ITEM x y z
            // -------------------------
            else if (command == "ITEM")
            {
                item = new Vector3D(ParseNumber(parts[1]),
                                    ParseNumber(parts[2]),
                                    ParseNumber(parts[3]));

                Console.WriteLine($"Item Position: {item}\n");
            }

            // -------------------------
            // WALL axis value
            // -------------------------
            else if (command == "WALL")
            {
                string axis = parts[1];
                double value = ParseNumber(parts[2]);
                Plane3D wall;

                if (axis == "X")
                {
                    wall = new Plane3D(1, 0, 0, value);
                }
                else if (axis == "Y")
                {
                    wall = new Plane3D(0, 1, 0, value);
                }
                else
                {
                    wall = new Plane3D(0, 0, 1, value);
                }

                walls.Add(wall);

                Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "Wall {0} Added: {1} = {2:F3}", walls.Count - 1, axis, value));
                Console.WriteLine($"  Plane: {wall}\n");
            }

            // -------------------------
            // WALL3 x1 y1 z1 x2 y2 z2 x3 y3 z3
            // -------------------------
            else if (command == "WALL3")
            {
                Vector3D p1 = new Vector3D(ParseNumber(parts[1]),
                                           ParseNumber(parts[2]),
                                           ParseNumber(parts[3]));
                Vector3D p2 = new Vector3D(ParseNumber(parts[4]),
                                           ParseNumber(parts[5]),
                                           ParseNumber(parts[6]));
                Vector3D p3 = new Vector3D(ParseNumber(parts[7]),
                                           ParseNumber(parts[8]),
                                           ParseNumber(parts[9]));

                Console.WriteLine("Wall (from three points) requested:");
                Console.WriteLine($"  P1 = {p1}, P2 = {p2}, P3 = {p3}");

                Plane3D wall = Plane3D.FromThreePoints(p1, p2, p3);

                if (wall == null)
                {
                    Console.WriteLine("  Wall could not be created; skipping.\n");
                }
                else
                {
                    walls.Add(wall);
                    Console.WriteLine($"  Wall {walls.Count - 1} Added: {wall}\n");
                }
            }

            // -------------------------
            // MOVE dx dy dz
            // -------------------------
            else if (command == "MOVE")
            {
                Vector3D movement = new Vector3D(ParseNumber(parts[1]),
                                                 ParseNumber(parts[2]),
                                                 ParseNumber(parts[3]));

                Console.WriteLine($"Attempted Move: {movement}");

                Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "  Movement magnitude: {0:F3}", movement.Magnitude()));

                Vector3D heading = movement.Normalize();

                if (heading != null)
                {
                    Console.WriteLine($"  Movement direction (unit vector): {heading}");
                }

                robot.Move(movement, walls);

                Vector3D displacement = robot.Position.Subtract(robot.StartPosition);

                Console.WriteLine($"  Displacement from start: {displacement}");

                // Item pickup detection
                if (item != null)
                {
                    Vector3D toItem = item.Subtract(robot.Position);

                    Console.WriteLine($"  Vector to item: {toItem}");

                    double distance = robot.Position.DistanceTo(item);

                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                        "  Distance to item: {0:F3}", distance));

                    double alignment = movement.Dot(toItem);

                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                        "  Alignment (move . to-item): {0:F3}", alignment));

                    // TODO: report whether the robot is moving toward the item,
                    // away from it, or perpendicular to it, based on the sign of
                    // the alignment value above.

                    // TODO: report whether the item has been retrieved.
                    // The item is retrieved when the robot is within
                    // PICKUP_RADIUS of it.
                }

                Console.WriteLine();
            }

            // -------------------------
            // ROUTE px py pz dx dy dz
            // -------------------------
            else if (command == "ROUTE")
            {
                route = new Line3D(
                    new Vector3D(ParseNumber(parts[1]),
                                 ParseNumber(parts[2]),
                                 ParseNumber(parts[3])),
                    new Vector3D(ParseNumber(parts[4]),
                                 ParseNumber(parts[5]),
                                 ParseNumber(parts[6]))
                );

                Console.WriteLine("Route Loaded.");
                Console.WriteLine($"  Point:     {route.Point}");
                Console.WriteLine($"  Direction: {route.Direction}\n");
            }

            // -------------------------
            // T t
            // -------------------------
            else if (command == "T")
            {
                double t = ParseNumber(parts[1]);

                Vector3D result = route.Evaluate(t);

                Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "Route evaluated at t = {0:F3}", t));
                Console.WriteLine($"  Position: {result}\n");
            }

            else
            {
                Console.WriteLine($"Unknown command: {command}\n");
            }
        }
    }

    public static void Main(string[] args)
    {
        string inputFile = (args.Length > 0) ? args[0] : "test_input.txt";

        ProcessFile(inputFile);
    }
}
