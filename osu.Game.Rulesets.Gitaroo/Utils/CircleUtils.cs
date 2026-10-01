using System;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.Utils;

public static class CircleUtils
{
    /// <summary>
    /// Get intersection points of a circle and a line
    /// </summary>
    /// <remarks>
    /// Formula from https://mathworld.wolfram.com/Circle-LineIntersection.html
    /// </remarks>
    /// <param name="a">A point of the line</param>
    /// <param name="b">Another point of the line</param>
    /// <param name="circleOrigin">The origin of the circle</param>
    /// <param name="r">The radius of the circle</param>
    /// <returns>An array of points (min 0, max 2)</returns>
    public static Vector2[] CircleLineIntersection(Vector2 a, Vector2 b, Vector2 circleOrigin, float r)
    {
        a -= circleOrigin;
        b -= circleOrigin;

        float dx = b.X - a.X;
        float dy = b.Y - a.Y;
        float dr = MathF.Sqrt(MathF.Pow(dx, 2) + MathF.Pow(dy, 2));
        float dr2 = MathF.Pow(dr, 2);
        float d = a.X * b.Y - b.X * a.Y;

        float delta = MathF.Pow(r, 2) * dr2 - MathF.Pow(d, 2);

        if (delta < 0) return [];

        float deltaSqrt = MathF.Sqrt(delta);

        Vector2[] points = new Vector2[2];

        float x1 = (d * dy + (dy < 0 ? -1 : 1) * dx * deltaSqrt) / dr2;
        float y1 = (-d * dx + MathF.Abs(dy) * deltaSqrt) / dr2;
        points[0] = new Vector2(x1, y1) + circleOrigin;

        float x2 = (d * dy - (dy < 0 ? -1 : 1) * dx * deltaSqrt) / dr2;
        float y2 = (-d * dx - MathF.Abs(dy) * deltaSqrt) / dr2;
        points[1] = new Vector2(x2, y2) + circleOrigin;

        return points;
    }

    /// <summary>
    /// Specialised <see cref="CircleLineIntersection"/> that return a single intersection,
    /// the one in the direction origin -> originDirection.
    /// </summary>
    /// <param name="origin">The origin</param>
    /// <param name="originDirection">The origin direction</param>
    /// <param name="circleOrigin">The circle origin</param>
    /// <param name="r">The radius of the circle</param>
    /// <returns>The intersection point (or null if the line don't cross the circle)</returns>
    public static Vector2? CircleLineIntersectionSingle(Vector2 origin, Vector2 originDirection, Vector2 circleOrigin, float r)
    {
        Vector2[] points = CircleLineIntersection(origin, originDirection, circleOrigin, r);

        if (points.Length == 0) return null;
        if (points.Length == 1) return points[0];

        Vector2 direction = originDirection - origin;

        Vector2 d1 = points[0] - origin;
        Vector2 d2 = points[1] - origin;

        float p1 = Vector2.Dot(d1, direction);
        float p2 = Vector2.Dot(d2, direction);

        return p1 > p2 ? points[0] : points[1];
    }
}
