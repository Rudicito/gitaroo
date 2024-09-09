// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.Gitaroo.Replays
{
    public class GitarooAutoGenerator : AutoGenerator<GitarooReplayFrame>
    {
        public new Beatmap<GitarooHitObject> Beatmap => (Beatmap<GitarooHitObject>)base.Beatmap;

        public GitarooAutoGenerator(IBeatmap beatmap)
            : base(beatmap)
        {
        }

        protected override void GenerateFrames()
        {
            Frames.Add(new GitarooReplayFrame());

            foreach (GitarooHitObject hitObject in Beatmap.HitObjects)
            {
                Frames.Add(new GitarooReplayFrame
                {
                    Time = hitObject.StartTime
                    // todo: add required inputs and extra frames.
                });
            }
        }
    }
}
