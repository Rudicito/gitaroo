// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.Gitaroo.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Gitaroo.Replays;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Play;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.UI;

[Cached]
public partial class DrawableGitarooRuleset : DrawableRuleset<GitarooHitObject>
{
    public new GitarooInputManager KeyBindingInputManager => (GitarooInputManager)base.KeyBindingInputManager;

    public new GitarooPlayfield Playfield => (GitarooPlayfield)base.Playfield;

    protected new GitarooRulesetConfigManager Config => (GitarooRulesetConfigManager)base.Config;

    public DrawableGitarooRuleset(GitarooRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod>? mods = null)
        : base(ruleset, beatmap, mods)
    {
    }

    protected override Playfield CreatePlayfield() => new GitarooPlayfield();

    protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new GitarooFramedReplayInputHandler(replay);

    public override DrawableHitObject<GitarooHitObject>? CreateDrawableRepresentation(GitarooHitObject h) => null;

    protected override PassThroughInputManager CreateInputManager() => new GitarooInputManager(Ruleset.RulesetInfo);

    protected override ResumeOverlay CreateResumeOverlay()
    {
        return new DelayedResumeOverlay { Scale = new Vector2(0.65f) };
    }
}
