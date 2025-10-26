using osu.Game.Rulesets.Gitaroo.Judgements;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Gitaroo.Objects;

public class TailNote : GitarooHitObject
{
    public const double RELEASE_WINDOW_LENIENCE = 36;

    public override Judgement CreateJudgement() => new HoldNoteTailJudgement();
    protected override HitWindows CreateHitWindows() => HitWindows.Empty;
}
