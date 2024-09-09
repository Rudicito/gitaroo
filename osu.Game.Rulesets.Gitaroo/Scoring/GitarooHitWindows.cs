using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Gitaroo.Scoring;

public class GitarooHitWindows : HitWindows
{
    public override bool IsHitResultAllowed(HitResult result)
    {
        switch (result)
        {
            case HitResult.Great:
            case HitResult.Good:
            case HitResult.Ok:
            case HitResult.Miss:
                return true;
        }

        return false;
    }
}
