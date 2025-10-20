// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.Gitaroo.Replays;

public class GitarooAutoGenerator : AutoGenerator<GitarooReplayFrame>
{
    public new Beatmap<GitarooHitObject> Beatmap => (Beatmap<GitarooHitObject>)base.Beatmap;

    public GitarooAutoGenerator(IBeatmap beatmap)
        : base(beatmap)
    {
    }

    protected override void GenerateFrames()
    {
        // Use to alternate
        bool hitButton = true;

        Frames.Add(new GitarooReplayFrame());

        foreach (GitarooHitObject hitObject in Beatmap.HitObjects)
        {
            switch (hitObject)
            {
                case Note:
                    addClickFrame(hitObject.StartTime, hitButton ? GitarooAction.LeftButton : GitarooAction.RightButton);
                    break;

                case HoldNote:
                    // TODO: Update when HoldNote gameplay going to change
                    addClickFrame(hitObject.StartTime, hitButton ? GitarooAction.LeftButton : GitarooAction.RightButton);
                    break;
            }

            hitButton = !hitButton;
        }
    }

    private void addClickFrame(double time, GitarooAction action)
    {
        Frames.Add(new GitarooReplayFrame(action) { Time = time });
        Frames.Add(new GitarooReplayFrame { Time = time + KEY_UP_DELAY }); // Release the keys as well
    }
}
