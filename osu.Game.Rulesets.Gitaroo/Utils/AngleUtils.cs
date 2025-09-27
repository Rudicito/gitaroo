using System;
using osu.Game.Rulesets.Objects;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.Utils;

public static class AngleUtils
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
    /// Checks if a given angle lies between two angles,
    /// properly handling wrap-around at 0/360 degrees.
    /// </summary>
    /// <param name="angle">The angle to test (in degrees).</param>
    /// <param name="start">Start of the range (in degrees).</param>
    /// <param name="end">End of the range (in degrees).</param>
    /// <returns><c>true</c> if angle is between start and end; otherwise, <c>false</c>.</returns>
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
        return NormalizeAngle(MathHelper.RadiansToDegrees(MathF.Atan2(direction.Y, direction.X)) + 90);
    }

    /// <summary>
    /// Moves a point from a starting position in a given direction by a specified distance.
    /// </summary>
    /// <param name="start">The initial position.</param>
    /// <param name="angleDegrees">The angle in degrees to move towards.</param>
    /// <param name="distance">The distance to move from the start position.</param>
    /// <returns>The new position after moving the specified distance at the given angle.</returns>
    public static Vector2 MovePoint(Vector2 start, float angleDegrees, float distance)
    {
        float angleRadians = MathHelper.DegreesToRadians(angleDegrees - 90);
        Vector2 direction = new Vector2((float)Math.Cos(angleRadians), (float)Math.Sin(angleRadians));
        return start + direction * distance;
    }

    /// <summary>
    /// Calculate the similarity of two angles.
    /// Returns 1 if they're exact, decreasing linearly towards 0 until maxError.
    /// </summary>
    /// <param name="value">The approximate angle</param>
    /// <param name="angle">The angle goal</param>
    /// <param name="maxError">The max error allowed</param>
    /// <returns>A value between 0 and 1</returns>
    public static float GetAngleCloseness(float value, float angle, float maxError)
    {
        value = NormalizeAngle(value);
        angle = NormalizeAngle(angle);

        float diff = Math.Abs(value - angle);

        // Handle wrap-around: take the smaller of the two possible angular diff
        if (diff > 180f)
            diff = 360f - diff;

        if (diff > maxError) return 0;

        return 1f - (diff / maxError);
    }

    /// <summary>
    /// Computes the angle (in degrees) of the <see cref="SliderPath"/> at a given progress.
    /// </summary>
    /// <param name="sliderPath">The <see cref="SliderPath"/>.</param>
    /// <param name="progress">Ranges from 0 (beginning of the path) to 1 (end of the path).</param>
    /// <param name="lookAhead">Distance to look ahead for calculating the direction. Defaults to a small value.</param>
    /// <returns>The angle in degrees, where 0 is pointing up.</returns>
    public static float AngleAtProgress(this SliderPath sliderPath, double progress, double lookAhead = 0.01)
    {
        Vector2 direction;
        Vector2 currentPos = sliderPath.PositionAt(progress);

        // In case progres == 1
        if (progress + lookAhead > 1)
        {
            Vector2 prevPos = sliderPath.PositionAt(Math.Clamp(progress - lookAhead, 0f, 1f));

            direction = currentPos - prevPos;
        }

        else
        {
            Vector2 nextPos = sliderPath.PositionAt(Math.Clamp(progress + lookAhead, 0f, 1f));

            direction = nextPos - currentPos;
        }

        return NormalizeAngle(MathHelper.RadiansToDegrees(MathF.Atan2(direction.Y, direction.X)) + 90);
    }
}
