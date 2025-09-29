using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Gitaroo.Utils;

public static class TraceLineUtils
{
    /// <summary>
    /// Scales the <see cref="TraceLine"/>'s path proportionally to match the expected distance,
    /// calculated from <see cref="TraceLine.Velocity"/> and <see cref="TraceLine.Duration"/>.
    /// This preserves the path's angle changes while adjusting its length.
    /// </summary>
    /// <remarks>
    /// Equivalent to using <see cref="SliderPath.ExpectedDistance"/>, but the path is scaled rather than extended or shortened.
    /// </remarks>
    public static void ScaleToExpectedDistance(this TraceLine traceLine)
    {
        double expectedDistance = traceLine.Velocity * traceLine.Duration;
        double currentDistance = traceLine.Path.CalculatedDistance;

        double scaleToExpectedDistance = expectedDistance / currentDistance;

        traceLine.Path.Scale((float)scaleToExpectedDistance);
    }
}
