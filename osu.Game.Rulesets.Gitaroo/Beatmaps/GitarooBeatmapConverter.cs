// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Threading;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Rulesets.Gitaroo.Beatmaps;

public class GitarooBeatmapConverter : BeatmapConverter<GitarooHitObject>
{
    private readonly bool isForCurrentRuleset;

    public GitarooBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
        : base(beatmap, ruleset)
    {
        isForCurrentRuleset = beatmap.BeatmapInfo.Ruleset.Equals(ruleset.RulesetInfo);
    }

    // todo: Check for conversion types that should be supported (ie. Beatmap.HitObjects.Any(h => h is IHasXPosition))
    // https://github.com/ppy/osu/tree/master/osu.Game/Rulesets/Objects/Types
    public override bool CanConvert() => true;

    protected override Beatmap<GitarooHitObject> CreateBeatmap()
    {
        return new GitarooBeatmap();
    }

    protected override Beatmap<GitarooHitObject> ConvertBeatmap(IBeatmap original, CancellationToken cancellationToken)
    {
        var beatmap = (GitarooBeatmap)base.ConvertBeatmap(original, cancellationToken);

        if (!isForCurrentRuleset)
        {
            beatmap.HitObjects.AddRange(generateTraceLine(beatmap));
        }

        // Can be more optimized?
        beatmap.HitObjects.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));

        return beatmap;
    }

    protected override IEnumerable<GitarooHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap, CancellationToken cancellationToken)
    {
        // if ((original as IHasCombo)?.NewCombo ?? false)
        //     patternGenerator.StartNextPattern();

        if (isForCurrentRuleset)
        {
            switch (original)
            {
                case GitarooHitObject gitarooObj:
                    return gitarooObj.Yield();

                default:
                    return [];
            }
        }

        else
        {
            switch (original)
            {
                case GitarooHitObject gitarooObj:
                {
                    return gitarooObj.Yield();
                }

                // If a osu Slider
                case IHasPathWithRepeats slider:
                    return new HoldNote
                    {
                        Samples = original.Samples,
                        StartTime = original.StartTime,
                        Duration = slider.Duration,
                    }.Yield();

                // If a osu Spinner
                case IHasDuration spinner:
                    return new HoldNote
                    {
                        Samples = original.Samples,
                        StartTime = original.StartTime,
                        Duration = spinner.Duration,
                    }.Yield();

                // If a osu HitCircle
                default:
                    return new HoldNote
                    {
                        Samples = original.Samples,
                        StartTime = original.StartTime,
                        Duration = 100,
                    }.Yield();
            }
        }
    }

    private List<TraceLine> generateTraceLine(GitarooBeatmap beatmap)
    {
        double velocity = beatmap.Difficulty.SliderMultiplier;
        // todo: Add TraceLine generator algorithm
        List<TraceLine> traceLines =
        [
            new TraceLine
            {
                Velocity = velocity,
                StartTime = 1000,
                EndTime = 10000,
                Path = new SliderPath(),
            }
        ];
        return traceLines;
    }
}
