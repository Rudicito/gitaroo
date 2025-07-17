using NUnit.Framework;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.Tests;

[TestFixture]
public partial class TestSceneTraceLine : DrawableGitarooRulesetTestScene
{
    private const double default_duration = 3000;
    private const float max_line_trace_length = 200;
    private const double delay = 1000;

    [Test]
    public void TestTraceLine()
    {
        AddStep("1 TraceLine with 1 HoLdNote", () => addTraceLineAndHoldNote());
    }

    private void addTraceLineAndHoldNote(double duration = default_duration)
    {
        DrawableRuleset.Playfield.Add(new TraceLine
        {
            StartTime = DrawableRuleset.Playfield.Time.Current + delay,
            EndTime = DrawableRuleset.Playfield.Time.Current + delay + duration,
            Velocity = 0.2,
            Path = new SliderPath(PathType.BEZIER, new[]
            {
                Vector2.Zero,
                new Vector2(-max_line_trace_length * 0.375f, max_line_trace_length * 0.18f),
                new Vector2(-max_line_trace_length / 2, 0),
                new Vector2(-max_line_trace_length * 0.75f, -max_line_trace_length / 2),
                new Vector2(-max_line_trace_length * 0.95f, 0),
                new Vector2(-max_line_trace_length, 0)
            }),
        });

        DrawableRuleset.Playfield.Add(new HoldNote
        {
            StartTime = DrawableRuleset.Playfield.Time.Current + delay + duration / 4,
            EndTime = DrawableRuleset.Playfield.Time.Current + delay + duration - duration / 4,
        });
    }
}
