using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.Tests;

[TestFixture]
public partial class TestSceneHitObjects : DrawableGitarooRulesetTestScene
{
    private const double default_duration = 3000;
    private const float max_line_trace_length = 200;
    private const double delay = 1000;

    private readonly SliderPath bezier = new SliderPath(PathType.BEZIER, new[]
    {
        Vector2.Zero,
        new Vector2(-max_line_trace_length * 0.375f, max_line_trace_length * 0.18f),
        new Vector2(-max_line_trace_length / 2, 0),
        new Vector2(-max_line_trace_length * 0.75f, -max_line_trace_length / 2),
        new Vector2(-max_line_trace_length * 0.95f, 0),
        new Vector2(-max_line_trace_length, 0)
    });

    private readonly SliderPath linearToBottomLeft = new SliderPath(PathType.LINEAR, new[]
    {
        Vector2.Zero,
        new Vector2(max_line_trace_length)
    });

    private readonly SliderPath linearToTopRight = new SliderPath(PathType.LINEAR, new[]
    {
        Vector2.Zero,
        new Vector2(-max_line_trace_length)
    });

    [Test]
    public void TestHitObjects()
    {
        AddStep("1 TraceLine, 1 HoLdNote", () =>
        {
            addTraceLine(0, 2000, linearToBottomLeft);
            addHoldNote(250, 1750);
        });

        AddStep("1 TraceLine, 1 Note", () =>
        {
            addTraceLine(0, 2000, bezier);
            addNote(1000);
        });
    }

    private void addHoldNote(double start, double end)
    {
        add(new HoldNote
        {
            StartTime = currentTime + start + delay,
            EndTime = currentTime + end + delay
        });
    }

    private void addNote(double start)
    {
        add(new Note
        {
            StartTime = currentTime + start + delay,
        });
    }

    private void addTraceLine(double start, double end, SliderPath sliderPath, double velocity = 0.2)
    {
        add(new TraceLine
        {
            StartTime = currentTime + start + delay,
            EndTime = currentTime + end + delay,
            Velocity = velocity,
            Path = sliderPath
        });
    }

    private void add(GitarooHitObject hitObject, bool kiai = false)
    {
        var cpi = createControlPointInfo(kiai);
        var difficulty = new BeatmapDifficulty();

        hitObject.ApplyDefaults(cpi, difficulty);

        DrawableRuleset.Playfield.Add(hitObject);
    }

    private ControlPointInfo createControlPointInfo(bool kiai)
    {
        var cpi = new ControlPointInfo();
        cpi.Add(-10000, new EffectControlPoint { KiaiMode = kiai });
        return cpi;
    }

    private double currentTime => DrawableRuleset.Playfield.Time.Current;
}
