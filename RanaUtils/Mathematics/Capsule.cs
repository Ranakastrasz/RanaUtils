using System;
using System.Numerics;

namespace RanaUtils.Mathematics;

public readonly struct Capsule : IEquatable<Capsule>
{
    /// <summary>
    /// The coordinates of the two points defining the capsule.
    /// </summary>
    public readonly Vector2 Point1;
    public readonly Vector2 Point2;

    /// <summary>
    /// The length, in pixels, from the center of this Capsule to the edge.
    /// </summary>
    public readonly float Radius;

    public readonly float X1 => Point1.X;
    public readonly float Y1 => Point1.Y;
    public readonly float X2 => Point2.X;
    public readonly float Y2 => Point2.Y;

    public static Capsule Empty { get; } = new();

    public readonly bool IsEmpty => Radius == 0;

    public readonly float Top => Math.Min(Y1, Y2) - Radius;
    public readonly float Bottom => Math.Max(Y1, Y2) + Radius;
    public readonly float Left => Math.Min(X1, X2) - Radius;
    public readonly float Right => Math.Max(X1, X2) + Radius;

    public Capsule(int x1, int y1, int x2, int y2, int radius)
    {
        Point1 = new Vector2(x1, y1);
        Point2 = new Vector2(x2, y2);
        Radius = radius;
    }

    public Capsule(Vector2 point1, Vector2 point2, int radius)
    {
        Point1 = point1;
        Point2 = point2;
        Radius = radius;
    }

    public bool Intersects(Circle other)
    {
        Vector2 circleCenter = other.Point;
        Vector2 a = new(X1, Y1);
        Vector2 b = new(X2, Y2);

        Vector2 closestPoint = ClosestPointOnSegment(circleCenter, a, b);
        float distanceSquared = Vector2.DistanceSquared(circleCenter, closestPoint);
        float allowedSquared = (Radius + other.Radius) * (Radius + other.Radius);

        return distanceSquared <= allowedSquared;
    }

    public bool Intersects(Capsule other)
    {
        Vector2 a1 = new(X1, Y1);
        Vector2 a2 = new(X2, Y2);
        Vector2 b1 = new(other.X1, other.Y1);
        Vector2 b2 = new(other.X2, other.Y2);

        float radiusSum = Radius + other.Radius;
        float distanceSquared = DistanceSquaredBetweenSegments(a1, a2, b1, b2);

        return distanceSquared <= radiusSum * radiusSum;
    }

    private static Vector2 ClosestPointOnSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float lengthSquared = ab.LengthSquared();

        if (lengthSquared <= 0.0001f)
        {
            return a;
        }

        float t = Vector2.Dot(p - a, ab) / lengthSquared;
        t = Math.Clamp(t, 0f, 1f);

        return a + ab * t;
    }

    private static float DistanceSquaredBetweenSegments(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        Vector2 u = a2 - a1;
        Vector2 v = b2 - b1;
        Vector2 w = a1 - b1;

        float a = u.X * u.X + u.Y * u.Y;
        float b = u.X * v.X + u.Y * v.Y;
        float c = v.X * v.X + v.Y * v.Y;
        float d = u.X * w.X + u.Y * w.Y;
        float e = v.X * w.X + v.Y * w.Y;

        float denominator = a * c - b * b;

        if (MathF.Abs(denominator) < 0.0001f)
        {
            // Parallel or nearly parallel segments; compare the closest endpoints.
            float minA = MathF.Min(
                Vector2.DistanceSquared(ClosestPointOnSegment(a1, b1, b2), a1),
                Vector2.DistanceSquared(ClosestPointOnSegment(a2, b1, b2), a2));

            float minB = MathF.Min(
                Vector2.DistanceSquared(ClosestPointOnSegment(b1, a1, a2), b1),
                Vector2.DistanceSquared(ClosestPointOnSegment(b2, a1, a2), b2));

            return MathF.Min(minA, minB);
        }

        float s = (b * e - c * d) / denominator;
        float t = (a * e - b * d) / denominator;

        if (s < 0f)
        {
            s = 0f;
            t = Math.Clamp((v.X * w.X + v.Y * w.Y) / c, 0f, 1f);
        }
        else if (s > 1f)
        {
            s = 1f;
            t = Math.Clamp((v.X * (w.X + u.X) + v.Y * (w.Y + u.Y)) / c, 0f, 1f);
        }

        if (t < 0f)
        {
            t = 0f;
            s = Math.Clamp((u.X * w.X + u.Y * w.Y) / a, 0f, 1f);
        }
        else if (t > 1f)
        {
            t = 1f;
            s = Math.Clamp((u.X * (w.X + v.X) + u.Y * (w.Y + v.Y)) / a, 0f, 1f);
        }

        Vector2 closestPointA = a1 + u * s;
        Vector2 closestPointB = b1 + v * t;

        return Vector2.DistanceSquared(closestPointA, closestPointB);
    }

    public override readonly bool Equals(object? obj) => obj is Capsule other && Equals(other);

    public readonly bool Equals(Capsule other) =>
        X1 == other.X1 &&
        Y1 == other.Y1 &&
        X2 == other.X2 &&
        Y2 == other.Y2 &&
        Radius == other.Radius;

    public override readonly int GetHashCode() => HashCode.Combine(X1, Y1, X2, Y2, Radius);

    public static bool operator ==(Capsule lhs, Capsule rhs) => lhs.Equals(rhs);
    public static bool operator !=(Capsule lhs, Capsule rhs) => !lhs.Equals(rhs);
}
