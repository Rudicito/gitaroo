using System;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.MathUtils;

internal static class Angle
{
    /// <summary>
    /// Normalizes an angle to the range [0, 360).
    /// </summary>
    /// <param name="angle">The angle in degrees.</param>
    /// <returns>The normalized angle in [0, 360).</returns>
    public static float NormalizeAngle(float angle)
    {
        angle %= 360f;
        return angle < 0 ? angle + 360f : angle;
    }

    /// <summary>
    /// Checks if a given angle lies between two angles on a circle,
    /// properly handling wrap-around at 0/360 degrees.
    /// </summary>
    /// <param name="angle">The angle to test (in degrees).</param>
    /// <param name="start">Start of the range (in degrees).</param>
    /// <param name="end">End of the range (in degrees).</param>
    /// <returns><c>true</c> if angle is between start and end (clockwise); otherwise, <c>false</c>.</returns>
    public static bool IsAngleBetween(float angle, float start, float end)
    {
        angle = NormalizeAngle(angle);
        start = NormalizeAngle(start);
        end = NormalizeAngle(end);

        if (start < end)
            return angle >= start && angle <= end;
        else // Wrap-around case
            return angle >= start || angle <= end;
    }

    public static float GetDegreesFromPosition(Vector2 a, Vector2 b)
    {
        var direction = b - a;
        return NormalizeAngle(MathHelper.RadiansToDegrees(MathF.Atan2(direction.Y, direction.X)));
    }

    public static Vector2 MovePoint(Vector2 start, float angleDegrees, float distance)
    {
        float angleRadians = MathHelper.DegreesToRadians(angleDegrees);
        Vector2 direction = new Vector2((float)Math.Cos(angleRadians), (float)Math.Sin(angleRadians));
        return start + direction * distance;
    }
}
