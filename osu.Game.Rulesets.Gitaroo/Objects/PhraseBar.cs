using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Rulesets.Gitaroo.Objects;

public class PhraseBar : GitarooHitObject, IHasDuration
{
    public double EndTime => StartTime + Duration;
    public double Duration { get; set; }
}
