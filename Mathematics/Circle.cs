using System;
using System.Numerics;


namespace RanaUtils.Mathematics;

public readonly struct Circle: IEquatable<Circle>
{
    /// <summary>
    /// Gets the location of the center of this circle.
    /// </summary>
    public readonly Vector2 Point;
    /// <summary>
    /// The length, in pixels, from the center of this circle to the edge.
    /// </summary>
    public readonly float Radius;

    public readonly float X => Point.X;
    public readonly float Y => Point.Y;

    /// <summary>
    /// Gets a circle with X=0, Y=0, and Radius=0.
    /// </summary>
    public static Circle Empty { get; } = new(0f,0f,0f);

    /// <summary>
    /// Gets a value that indicates whether this circle has a radius of 0.
    /// </summary>
    public readonly bool IsEmpty => Radius == 0;

    /// <summary>
    /// Gets the y-coordinate of the highest point on this circle.
    /// </summary>
    public readonly float Top => Y - Radius;

    /// <summary>
    /// Gets the y-coordinate of the lowest point on this circle.
    /// </summary>
    public readonly float Bottom => Y + Radius;

    /// <summary>
    /// Gets the x-coordinate of the leftmost point on this circle.
    /// </summary>
    public readonly float Left => X - Radius;

    /// <summary>
    /// Gets the x-coordinate of the rightmost point on this circle.
    /// </summary>
    public readonly float Right => X + Radius;

    /// <summary>
    /// Creates a new circle with the specified position and radius.
    /// </summary>
    /// <param name="x">The x-coordinate of the center of the circle.</param>
    /// <param name="y">The y-coordinate of the center of the circle..</param>
    /// <param name="radius">The length from the center of the circle to an edge.</param>
    public Circle(float x, float y, float radius)
    {
        Point = new Vector2(x, y);
        Radius = radius;
    }

    /// <summary>
    /// Creates a new circle with the specified position and radius.
    /// </summary>
    /// <param name="point">The center of the circle.</param>
    /// <param name="radius">The length from the center of the circle to an edge.</param>
    public Circle(Vector2 point, float radius)
    {
        Point = point;
        Radius = radius;
    }

    /// <summary>
    /// Returns a value that indicates whether the specified circle intersects with this circle.
    /// </summary>
    /// <param name="other">The other circle to check.</param>
    /// <returns>true if the other circle intersects with this circle; otherwise, false.</returns>
    public bool Intersects(Circle other)
    {
        float radiiSquared = (Radius + other.Radius) * (Radius + other.Radius);
        float distanceSquared = Vector2.DistanceSquared(Point, other.Point);
        return distanceSquared < radiiSquared;
    }

    /// <summary>
    /// Returns a value that indicates whether this circle and the specified object are equal
    /// </summary>
    /// <param name="obj">The object to compare with this circle.</param>
    /// <returns>true if this circle and the specified object are equal; otherwise, false.</returns>
    public override readonly bool Equals(object? obj) => obj is Circle other && Equals(other);

    /// <summary>
    /// Returns a value that indicates whether this circle and the specified circle are equal.
    /// </summary>
    /// <param name="other">The circle to compare with this circle.</param>
    /// <returns>true if this circle and the specified circle are equal; otherwise, false.</returns>
    public readonly bool Equals(Circle other) => X == other.X &&
                                                    Y == other.Y &&
                                                    Radius == other.Radius;

    /// <summary>
    /// Returns the hash code for this circle.
    /// </summary>
    /// <returns>The hash code for this circle as a 32-bit signed integer.</returns>
    public override readonly int GetHashCode() => HashCode.Combine(X, Y, Radius);

    /// <summary>
    /// Returns a value that indicates if the circle on the left hand side of the equality operator is equal to the
    /// circle on the right hand side of the equality operator.
    /// </summary>
    /// <param name="lhs">The circle on the left hand side of the equality operator.</param>
    /// <param name="rhs">The circle on the right hand side of the equality operator.</param>
    /// <returns>true if the two circles are equal; otherwise, false.</returns>
    public static bool operator ==(Circle lhs, Circle rhs) => lhs.Equals(rhs);

    /// <summary>
    /// Returns a value that indicates if the circle on the left hand side of the inequality operator is not equal to the
    /// circle on the right hand side of the inequality operator.
    /// </summary>
    /// <param name="lhs">The circle on the left hand side of the inequality operator.</param>
    /// <param name="rhs">The circle on the right hand side fo the inequality operator.</param>
    /// <returns>true if the two circle are not equal; otherwise, false.</returns>
    public static bool operator !=(Circle lhs, Circle rhs) => !lhs.Equals(rhs);

}
