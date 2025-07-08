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
        AddStep("addLineTrace", () => addLineTrace());
    }

    private void addLineTrace(double duration = default_duration, bool kiai = false)
    {
        var d = new LineTrace()
        {
            StartTime = DrawableRuleset.Playfield.Time.Current,
            Duration = duration,
            Path = new SliderPath(PathType.LINEAR, new[]
            {
                Vector2.Zero,
                new Vector2(max_line_trace_length * 0.375f, max_line_trace_length * 0.18f),
                new Vector2(max_line_trace_length / 2, 0),
                new Vector2(max_line_trace_length * 0.75f, -max_line_trace_length / 2),
                new Vector2(max_line_trace_length * 0.95f, 0),
                new Vector2(max_line_trace_length, 0)
            }),
        };

        var cpi = new ControlPointInfo();
        cpi.Add(-10000, new EffectControlPoint { KiaiMode = kiai });

        d.ApplyDefaults(cpi, new BeatmapDifficulty());

        DrawableRuleset.Playfield.Add(new DrawableLineTrace(d));
    }
}
