using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Gitaroo.Objects;

public class LineTrace : GitarooHitObject, IHasPath
{
    public LineTrace()
    {
    }

    public override Judgement CreateJudgement() => new IgnoreJudgement();
    protected override HitWindows? CreateHitWindows() => null;

    public double EndTime
    {
        get => StartTime + Duration;
        set => Duration = value - StartTime;
    }

    public double Duration { get; set; }
    public double Distance => Path.Distance;
    public required SliderPath Path { get; set; }
}
