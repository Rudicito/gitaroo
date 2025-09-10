using System.Threading;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;

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

    /// <summary>
    /// The head note of the hold.
    /// </summary>
    public HeadNote Head { get; protected set; } = null!;

    protected override void CreateNestedHitObjects(CancellationToken cancellationToken)
    {
        base.CreateNestedHitObjects(cancellationToken);

        AddNested(Head = new HeadNote
        {
            StartTime = StartTime,
        });
    }

    public override Judgement CreateJudgement() => new IgnoreJudgement();
    protected override HitWindows? CreateHitWindows() => null;
}
