using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Gitaroo.Judgements;

public class HoldNoteTailJudgement : GitarooJudgement
{
    public override HitResult MaxResult => HitResult.IgnoreHit;
    public override HitResult MinResult => HitResult.IgnoreMiss;
}
