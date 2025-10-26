using osu.Game.Rulesets.Gitaroo.Judgements;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Gitaroo.Objects;

/// <summary>
/// The body of a <see cref="HoldNote"/>.
/// Mostly a dummy hitobject that provides the judgement for the "holding" state.<br />
/// On hit - the hold note was held correctly for the full duration.<br />
/// On miss - the hold note was released at some point during its judgement period.
/// </summary>
public class HoldNoteBody : GitarooHitObject
{
    public override Judgement CreateJudgement() => new HoldNoteBodyJudgement();
    protected override HitWindows CreateHitWindows() => HitWindows.Empty;
}
