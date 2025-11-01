// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Threading;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Gitaroo.Utils;
using osu.Game.Rulesets.Objects.Types;
using osuTK;

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
            var comboData = original as IHasCombo;

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
                        NodeSamples = slider.NodeSamples,
                        StartTime = original.StartTime,
                        Duration = slider.Duration,
                        NewCombo = comboData?.NewCombo ?? false,
                        ComboOffset = comboData?.ComboOffset ?? 0,
                    }.Yield();

                // If a osu Spinner
                case IHasDuration spinner:
                    return new HoldNote
                    {
                        Samples = original.Samples,
                        StartTime = original.StartTime,
                        Duration = spinner.Duration,
                        NewCombo = comboData?.NewCombo ?? false,
                        ComboOffset = comboData?.ComboOffset ?? 0,
                        WasSpinner = true,
                    }.Yield();

                // If a osu HitCircle
                default:
                    return new Note
                    {
                        Samples = original.Samples,
                        StartTime = original.StartTime,
                        NewCombo = comboData?.NewCombo ?? false,
                        ComboOffset = comboData?.ComboOffset ?? 0,
                    }.Yield();
            }
        }
    }

    private List<TraceLine> generateTraceLine(GitarooBeatmap beatmap)
    {
        double velocity = beatmap.Difficulty.SliderMultiplier / 5;
        // todo: Do a much better TraceLine generator algorithm

        const float max_line_trace_length = 1000;

        SliderPath sliderPath = new SliderPath(PathType.PERFECT_CURVE, new[]
        {
            Vector2.Zero,
            new Vector2(max_line_trace_length, 0),
            new Vector2(max_line_trace_length, max_line_trace_length)
        });

        (double start, double end) = beatmap.CalculatePlayableBounds();

        List<TraceLine> traceLines =
        [
            new TraceLine
            {
                StartTime = start - 1000,
                EndTime = end + 10000,
                Velocity = velocity,
                Path = sliderPath,
            }
        ];

        foreach (var traceLine in traceLines)
        {
            traceLine.ScaleToExpectedDistance();
        }

        return traceLines;
    }
}
