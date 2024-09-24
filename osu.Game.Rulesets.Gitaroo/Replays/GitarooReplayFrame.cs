// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.Gitaroo.Replays;

public class GitarooReplayFrame : ReplayFrame
{
    public List<GitarooAction> Actions = new List<GitarooAction>();

    public GitarooReplayFrame(GitarooAction? button = null)
    {
        if (button.HasValue)
            Actions.Add(button.Value);
    }
}
