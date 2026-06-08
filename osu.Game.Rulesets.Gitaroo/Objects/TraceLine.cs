using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Rulesets.Gitaroo.Objects;

/// <summary>
/// The path where <see cref="TraceLineHitObject"/> are placed into, that the FanShaped must follow.
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

    private readonly SliderPath path = new SliderPath();

    public SliderPath Path
    {
        get => path;
        set
        {
            path.ControlPoints.Clear();
            path.ControlPoints.AddRange(value.ControlPoints.Select(c => new PathControlPoint(c.Position, c.Type)));
        }
    }

    public List<(double progress, float length)> Segments { get; set; } = [];

    private readonly GitarooSliderPath convertedPath = new GitarooSliderPath();

    public GitarooSliderPath ConvertedPath
    {
        get => convertedPath;
        set
        {
            convertedPath.ControlPoints.Clear();
            convertedPath.ControlPoints.AddRange(value.ControlPoints.Select(c => new PathControlPoint(c.Position, c.Type)));
        }
    }
}
