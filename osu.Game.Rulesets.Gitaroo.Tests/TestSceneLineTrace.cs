using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Gitaroo.Objects.Drawables;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.Tests;

[TestFixture]
public partial class TestSceneLineTrace : DrawableGitarooRulesetTestScene
{
    private const double default_duration = 3000;
    private const float max_line_trace_length = 200;

    [Test]
    public void TestLineTrace()
    {
        AddStep("addLineTrace", () => addHitObjects());
    }

    private void addHitObjects(double duration = default_duration, bool kiai = false)
    {
        // Create control point info once
        var cpi = createControlPointInfo(kiai);
        var difficulty = new BeatmapDifficulty();
        double currentTime = DrawableRuleset.Playfield.Time.Current;

        // Add LineTrace
        var lineTrace = createLineTrace(currentTime + 5000, duration);
        lineTrace.ApplyDefaults(cpi, difficulty);
        DrawableRuleset.Playfield.Add(new DrawableLineTrace(lineTrace));

        // Add HoldNote
        var holdNote = createHoldNote(currentTime + 6000, 1000);
        holdNote.ApplyDefaults(cpi, difficulty);
        DrawableRuleset.Playfield.Add(new DrawableHoldNote(holdNote));
    }

    private ControlPointInfo createControlPointInfo(bool kiai)
    {
        var cpi = new ControlPointInfo();
        cpi.Add(-10000, new EffectControlPoint { KiaiMode = kiai });
        return cpi;
    }

    private LineTrace createLineTrace(double startTime, double duration)
    {
        return new LineTrace
        {
            Velocity = 0.2,
            StartTime = startTime,
            Duration = duration,
            Path = new SliderPath(PathType.BEZIER, new[]
            {
                Vector2.Zero,
                new Vector2(max_line_trace_length * 0.375f, max_line_trace_length * 0.18f),
                new Vector2(max_line_trace_length / 2, 0),
                new Vector2(max_line_trace_length * 0.75f, -max_line_trace_length / 2),
                new Vector2(max_line_trace_length * 0.95f, 0),
                new Vector2(max_line_trace_length, 0)
            }),
        };
    }

    private HoldNote createHoldNote(double startTime, double duration)
    {
        return new HoldNote
        {
            StartTime = startTime,
            Duration = duration,
        };
    }
}
