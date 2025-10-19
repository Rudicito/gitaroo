using NUnit.Framework;

namespace osu.Game.Rulesets.Gitaroo.Tests;

[TestFixture]
public partial class TestSceneHitObjects : DrawableGitarooRulesetTestScene
{
    protected override void LoadComplete()
    {
        base.LoadComplete();

        AddSliderStep("TraceLine Velocity", 0.1, 1, DEFAULT_TRACE_LINE_VELOCITY, v => TraceLineVelocity = v);
        AddSliderStep("Spawn delay", 0, 5000, DEFAULT_DELAY, v => Delay = v);
    }

    [Test]
    public void TestLinear()
    {
        AddStep("1 TraceLine, 1 HoLdNote", () =>
        {
            KillAll();
            AddTraceLine(0, 2000, LinearBottomRightPath);
            AddHoldNote(250, 1750);
        });

        AddStep("1 TraceLine, 1 Note", () =>
        {
            KillAll();
            AddTraceLine(0, 2000, PerfectCurvePath);
            AddNote(1000);
        });
    }

    [Test]
    public void TestBezier()
    {
        AddStep("1 TraceLine, 1 HoLdNote", () =>
        {
            KillAll();
            AddTraceLine(0, 2000, BezierPath);
            AddHoldNote(250, 1750);
        });

        AddStep("1 TraceLine, 1 Note", () =>
        {
            KillAll();
            AddTraceLine(0, 2000, BezierPath);
            AddNote(1000);
        });
    }

    [Test]
    public void TestPerfectCurve()
    {
        AddStep("1 TraceLine, 1 HoLdNote", () =>
        {
            KillAll();
            AddTraceLine(0, 2000, PerfectCurvePath);
            AddHoldNote(250, 1750);
        });

        AddStep("1 TraceLine, 1 Note", () =>
        {
            KillAll();
            AddTraceLine(0, 2000, PerfectCurvePath);
            AddNote(1000);
        });
    }

    private double traceLineDuration;

    [Test]
    public void TestHitObjectsManual()
    {
        AddSliderStep("TraceLine Duration", 1000, 100000, 10000, v => traceLineDuration = v);

        AddStep("Kill all HitObjects", KillAll);

        AddStep("Add TraceLine Linear", () =>
        {
            AddTraceLine(0, traceLineDuration, LinearBottomRightPath);
        });

        AddStep("Add TraceLine Bezier", () =>
        {
            AddTraceLine(0, traceLineDuration, BezierPath);
        });

        AddStep("Add TraceLine PerfectCurve", () =>
        {
            AddTraceLine(0, traceLineDuration, PerfectCurvePath);
        });

        AddStep("Add Note", () =>
        {
            AddNote(0);
        });

        AddStep("Add HoldNote", () =>
        {
            AddHoldNote(0, 500);
        });
    }
}
