// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Threading;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Gitaroo.UI;
using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Rulesets.Gitaroo.Beatmaps;

public class GitarooBeatmapConverter : BeatmapConverter<GitarooHitObject>
{
    public GitarooBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
        : base(beatmap, ruleset)
    {
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
        beatmap.LineTrace = generateLineTrace(beatmap);
        return beatmap;
    }

    protected override IEnumerable<GitarooHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap, CancellationToken cancellationToken)
    {
        // if ((original as IHasCombo)?.NewCombo ?? false)
        //     patternGenerator.StartNextPattern();

        switch (original)
        {
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
                return new Note
                {
                    Samples = original.Samples,
                    StartTime = original.StartTime,
                }.Yield();
        }
    }

    private LineTrace generateLineTrace(GitarooBeatmap beatmap)
    {
        // todo: Add LineTrace generator algorithm
        return new LineTrace();
    }
}
