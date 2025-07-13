using System;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Gitaroo.Scoring;

public class GitarooHitWindows : HitWindows
{
    private static readonly DifficultyRange great_window_range = new(50, 35, 20);
    private static readonly DifficultyRange good_window_range = new(70, 50, 30);
    private static readonly DifficultyRange meh_window_range = new(100, 70, 50);
    private static readonly DifficultyRange miss_window_range = new(120, 85, 60);

    private double great;
    private double good;
    private double meh;
    private double miss;

    public override bool IsHitResultAllowed(HitResult result)
    {
        switch (result)
        {
            case HitResult.Great:
            case HitResult.Good:
            case HitResult.Meh:
            case HitResult.Miss:
                return true;
        }

        return false;
    }

    public override void SetDifficulty(double difficulty)
    {
        great = IBeatmapDifficultyInfo.DifficultyRange(difficulty, great_window_range);
        good = IBeatmapDifficultyInfo.DifficultyRange(difficulty, good_window_range);
        meh = IBeatmapDifficultyInfo.DifficultyRange(difficulty, meh_window_range);
        miss = IBeatmapDifficultyInfo.DifficultyRange(difficulty, miss_window_range);
    }

    public override double WindowFor(HitResult result)
    {
        switch (result)
        {
            case HitResult.Great:
                return great;

            case HitResult.Good:
                return good;

            case HitResult.Meh:
                return meh;

            case HitResult.Miss:
                return miss;

            default:
                throw new ArgumentOutOfRangeException(nameof(result), result, null);
        }
    }
}
