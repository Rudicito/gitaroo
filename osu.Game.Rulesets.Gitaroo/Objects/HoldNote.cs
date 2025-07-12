using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Rulesets.Gitaroo.Objects;

public class HoldNote : GitarooHitObject, IHasDuration
{
    public double EndTime => StartTime + Duration;
    public double Duration { get; set; }

    public double VelocityMultiplier { get; set; } = 1;
}
