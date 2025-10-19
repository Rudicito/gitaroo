using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Gitaroo.UI;
using osu.Game.Rulesets.Gitaroo.Utils;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.Tests;

public abstract partial class DrawableGitarooRulesetTestScene : TestSceneOsuGitaroo
{
    public const int DEFAULT_PLAYFIELD_CONTAINER_HEIGHT = 768;

    protected DrawableGitarooRuleset DrawableRuleset = null!;
    protected Container PlayfieldContainer = null!;

    private ControlPointInfo controlPointInfo = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        controlPointInfo = new ControlPointInfo();
        // controlPointInfo.Add(-10000, new EffectControlPoint());
        controlPointInfo.Add(0, new TimingControlPoint());

        IWorkingBeatmap beatmap = CreateWorkingBeatmap(CreateBeatmap(new GitarooRuleset().RulesetInfo));

        Add(PlayfieldContainer = new Container
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            RelativeSizeAxes = Axes.X,
            Height = DEFAULT_PLAYFIELD_CONTAINER_HEIGHT,
            Children = new[] { DrawableRuleset = new DrawableGitarooRuleset(new GitarooRuleset(), beatmap.GetPlayableBeatmap(beatmap.BeatmapInfo.Ruleset)) }
        });
    }

    protected override IBeatmap CreateBeatmap(RulesetInfo ruleset)
    {
        return new Beatmap
        {
            HitObjects = new List<HitObject>(),
            BeatmapInfo = new BeatmapInfo
            {
                Difficulty = new BeatmapDifficulty(),
                Metadata = new BeatmapMetadata
                {
                    Artist = @"Unknown",
                    Title = @"Sample Beatmap",
                    Author = { Username = @"peppy" },
                },
                Ruleset = ruleset
            },
            ControlPointInfo = controlPointInfo
        };
    }

    public const float MAX_LINE_TRACE_LENGTH = 200;

    public const double DEFAULT_DELAY = 1000;
    public double Delay = DEFAULT_DELAY;

    public const double DEFAULT_TRACE_LINE_VELOCITY = 0.2;
    public double TraceLineVelocity = DEFAULT_TRACE_LINE_VELOCITY;

    public readonly SliderPath BezierPath = new SliderPath(PathType.BEZIER, new[]
    {
        Vector2.Zero,
        new Vector2(-MAX_LINE_TRACE_LENGTH * 0.375f, MAX_LINE_TRACE_LENGTH * 0.18f),
        new Vector2(-MAX_LINE_TRACE_LENGTH / 2, 0),
        new Vector2(-MAX_LINE_TRACE_LENGTH * 0.75f, -MAX_LINE_TRACE_LENGTH / 2),
        new Vector2(-MAX_LINE_TRACE_LENGTH * 0.95f, 0),
        new Vector2(-MAX_LINE_TRACE_LENGTH, 0)
    });

    public readonly SliderPath LinearBottomRightPath = new SliderPath(PathType.LINEAR, new[]
    {
        Vector2.Zero,
        new Vector2(MAX_LINE_TRACE_LENGTH)
    });

    public readonly SliderPath PerfectCurvePath = new SliderPath(PathType.PERFECT_CURVE, new[]
    {
        Vector2.Zero,
        new Vector2(MAX_LINE_TRACE_LENGTH, 0),
        new Vector2(MAX_LINE_TRACE_LENGTH, MAX_LINE_TRACE_LENGTH)
    });

    protected void AddHoldNote(double start, double end)
    {
        Add(new HoldNote
        {
            StartTime = CurrentTime + start + Delay,
            EndTime = CurrentTime + end + Delay
        });
    }

    protected void AddNote(double start)
    {
        Add(new Note
        {
            StartTime = CurrentTime + start + Delay,
        });
    }

    protected void AddTraceLine(double start, double end, SliderPath sliderPath, double? velocity = null)
    {
        velocity ??= TraceLineVelocity;

        var traceLine = new TraceLine
        {
            StartTime = CurrentTime + start + Delay,
            EndTime = CurrentTime + end + Delay,
            Velocity = velocity.Value,
            Path = sliderPath
        };

        traceLine.ScaleToExpectedDistance();

        Add(traceLine);
    }

    protected void Add(GitarooHitObject hitObject, bool kiai = false)
    {
        var cpi = CreateControlPointInfo(kiai);
        var difficulty = new BeatmapDifficulty();

        hitObject.ApplyDefaults(cpi, difficulty);

        DrawableRuleset.Playfield.Add(hitObject);
    }

    protected void KillAll()
    {
        DrawableRuleset.Playfield.AllHitObjects.ForEach(dho => DrawableRuleset.Playfield.Remove(dho));
    }

    // TODO: Kiai not working in test
    protected ControlPointInfo CreateControlPointInfo(bool kiai)
    {
        var cpi = new ControlPointInfo();
        cpi.Add(-10000, new EffectControlPoint { KiaiMode = kiai });
        return cpi;
    }

    public double CurrentTime => DrawableRuleset.Playfield.Time.Current;
}
