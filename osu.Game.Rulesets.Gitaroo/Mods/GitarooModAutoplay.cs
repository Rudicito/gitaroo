// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Gitaroo.Replays;
using osu.Game.Rulesets.Gitaroo.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Gitaroo.Mods;

public class GitarooModAutoplay : ModAutoplay, IApplicableToDrawableRuleset<GitarooHitObject>
{
    public override ModReplayData CreateReplayData(IBeatmap beatmap, IReadOnlyList<Mod> mods)
        => new ModReplayData(new GitarooAutoGenerator(beatmap).Generate(), new ModCreatedUser { Username = "Puma" });

    public void ApplyToDrawableRuleset(DrawableRuleset<GitarooHitObject> drawableRuleset)
    {
        var gitarooRuleset = (DrawableGitarooRuleset)drawableRuleset;

        gitarooRuleset.Playfield.FanShaped.Auto = true;
    }
}
