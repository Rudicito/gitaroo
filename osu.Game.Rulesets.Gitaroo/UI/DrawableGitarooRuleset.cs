// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Gitaroo.Objects.Drawables;
using osu.Game.Rulesets.Gitaroo.Replays;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.UI.Scrolling;

namespace osu.Game.Rulesets.Gitaroo.UI;

[Cached]
public partial class DrawableGitarooRuleset : DrawableScrollingRuleset<GitarooHitObject>
{
    public DrawableGitarooRuleset(GitarooRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
        : base(ruleset, beatmap, mods)
    {
        Direction.Value = ScrollingDirection.Left;
        TimeRange.Value = 6000;
    }

    protected override Playfield CreatePlayfield() => new GitarooPlayfield();

    protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new GitarooFramedReplayInputHandler(replay);

    public override DrawableHitObject<GitarooHitObject> CreateDrawableRepresentation(GitarooHitObject h) => new DrawableGitarooHitObject(h);

    protected override PassThroughInputManager CreateInputManager() => new GitarooInputManager(Ruleset?.RulesetInfo);
}
