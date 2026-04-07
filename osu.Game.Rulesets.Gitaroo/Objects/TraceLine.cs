using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Rulesets.Gitaroo.Objects;

/// <summary>
/// The line where <see cref="Note"/> and <see cref="HoldNote"/> are placed into, that the FanShaped must follow.
/// </summary>
public class TraceLine : GitarooHitObject, IHasPath
{
    public override Judgement CreateJudgement() => new IgnoreJudgement();

    public double EndTime
    {
        get => StartTime + Duration;
        set => Duration = value - StartTime;
    }

    public double Duration { get; set; }
    public double Distance => Path.Distance;

    public required SliderPath Path { get; set; } = null!;
}
