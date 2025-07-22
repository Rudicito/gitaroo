using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Rulesets.Gitaroo.Objects;

/// <summary>
/// Represents a hit object which requires press and holding a key.
/// </summary>
public class HoldNote : GitarooHitObject, IHasDuration
{
    public double EndTime
    {
        get => StartTime + Duration;
        set => Duration = value - StartTime;
    }

    public double Duration { get; set; }

    public double VelocityMultiplier { get; set; } = 1;
}
