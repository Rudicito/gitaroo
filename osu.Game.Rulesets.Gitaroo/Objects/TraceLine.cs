using System;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Gitaroo.Objects;

/// <summary>
/// The line where <see cref="Note"/> and <see cref="HoldNote"/> are placed into, that the FanShaped must follow.
/// </summary>
public class TraceLine : GitarooHitObject, IHasPath
{
    public TraceLine()
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

    private SliderPath path = null!;

    public required SliderPath Path
    {
        get => path;
        set
        {
            path = value;

            if (Velocity == 0) throw new InvalidOperationException("TraceLine Velocity cannot be 0");

            path.ExpectedDistance.Value = Velocity * Duration;
        }
    }

    public required double Velocity { get; set; }
}
