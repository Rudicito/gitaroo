using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Gitaroo.Objects;

namespace osu.Game.Rulesets.Gitaroo.Beatmaps;

public class GitarooBeatmap : Beatmap<GitarooHitObject>
{
    public override IEnumerable<BeatmapStatistic> GetStatistics()
    {
        int notes = HitObjects.Count(n => n is Note);
        int holdNotes = HitObjects.Count(h => h is HoldNote);
        int sum = Math.Max(1, notes + holdNotes);

        int traceLines = HitObjects.Count(s => s is TraceLine);

        return new[]
        {
            new BeatmapStatistic
            {
                Name = @"Notes",
                CreateIcon = () => new BeatmapStatisticIcon(BeatmapStatisticsIconType.Circles),
                Content = notes.ToString(),
                BarDisplayLength = notes / (float)sum,
            },
            new BeatmapStatistic
            {
                Name = @"Hold Notes",
                CreateIcon = () => new BeatmapStatisticIcon(BeatmapStatisticsIconType.Sliders),
                Content = holdNotes.ToString(),
                BarDisplayLength = holdNotes / (float)sum,
            },
            new BeatmapStatistic
            {
                Name = @"Trace Lines",
                CreateIcon = () => new BeatmapStatisticIcon(BeatmapStatisticsIconType.Sliders),
                Content = traceLines.ToString(),
            }
        };
    }
}
