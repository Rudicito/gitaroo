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
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="circleCoords"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public static Vector2[] CircleLineIntersection(Vector2 a, Vector2 b, Vector2 circleCoords, float r)
    {
        a -= circleCoords;
        b -= circleCoords;

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
        points[0] = new Vector2(x1, y1) + circleCoords;

        float x2 = (d * dy - (dy < 0 ? -1 : 1) * dx * deltaSqrt) / dr2;
        float y2 = (-d * dx - MathF.Abs(dy) * deltaSqrt) / dr2;
        points[1] = new Vector2(x2, y2) + circleCoords;

        return points;
    }

    public static Vector2? CircleLineIntersectionSingle(Vector2 a, Vector2 b, Vector2 c, float r, Vector2 origin)
    {
        Vector2[] points = CircleLineIntersection(a, b, c, r);

        if (points.Length == 0) return null;
        if (points.Length == 1) return points[0];

        Vector2 direction = b - a;

        Vector2 d1 = points[0] - origin;
        Vector2 d2 = points[1] - origin;

        float p1 = Vector2.Dot(d1, direction);
        float p2 = Vector2.Dot(d2, direction);

        return p1 < p2 ? points[0] : points[1];
    }
}
