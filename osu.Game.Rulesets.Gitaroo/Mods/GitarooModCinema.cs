// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Gitaroo.Replays;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Gitaroo.Mods;

public class GitarooModCinema : ModCinema<GitarooHitObject>
{
    public override Type[] IncompatibleMods => base.IncompatibleMods.Concat(new[] { typeof(GitarooModAutopilot) }).ToArray();

    public override ModReplayData CreateReplayData(IBeatmap beatmap, IReadOnlyList<Mod> mods)
        => new ModReplayData(new GitarooAutoGenerator(beatmap).Generate(), new ModCreatedUser { Username = "Puma" });
}
