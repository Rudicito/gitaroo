using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Gitaroo.Judgements;

public class HoldNoteBodyJudgement : GitarooJudgement
{
    public override HitResult MaxResult => HitResult.IgnoreHit;
    public override HitResult MinResult => HitResult.ComboBreak;
}
