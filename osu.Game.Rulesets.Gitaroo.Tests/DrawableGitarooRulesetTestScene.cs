using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Gitaroo.UI;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Gitaroo.Tests;

public abstract partial class DrawableGitarooRulesetTestScene : TestSceneOsuGitaroo
{
    protected const int DEFAULT_PLAYFIELD_CONTAINER_HEIGHT = 768;

    protected DrawableGitarooRuleset DrawableRuleset { get; private set; }
    protected Container PlayfieldContainer { get; private set; }

    private ControlPointInfo controlPointInfo { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        controlPointInfo = new ControlPointInfo();
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
            HitObjects = new List<HitObject> { new Note() },
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
}
