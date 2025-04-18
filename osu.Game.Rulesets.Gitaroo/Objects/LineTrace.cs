using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Gitaroo.Objects;

public class LineTrace : GitarooHitObject
{
    public override Judgement CreateJudgement() => new IgnoreJudgement();
    protected override HitWindows CreateHitWindows() => null;
}
