using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Gitaroo.Objects;

namespace osu.Game.Rulesets.Gitaroo.Beatmaps;

public class GitarooBeatmap : Beatmap<GitarooHitObject>
{
    public override IEnumerable<BeatmapStatistic> GetStatistics()
    {
        int notes = HitObjects.Count(s => s is Note);
        int holdNotes = HitObjects.Count(s => s is HoldNote);

        return new[]
        {
            new BeatmapStatistic
            {
                Name = @"Notes",
                CreateIcon = () => new BeatmapStatisticIcon(BeatmapStatisticsIconType.Circles),
                Content = notes.ToString(),
            },
            new BeatmapStatistic
            {
                Name = @"Hold Notes",
                CreateIcon = () => new BeatmapStatisticIcon(BeatmapStatisticsIconType.Sliders),
                Content = holdNotes.ToString(),
            },
        };
    }
}
