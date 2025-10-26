namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public partial class DrawableHoldNoteBody : DrawableTraceLineHitObject<HoldNoteBody>
{
    public override bool DisplayResult => false;

    public DrawableHoldNoteBody()
        : this(null)
    {
    }

    public DrawableHoldNoteBody(HoldNoteBody? hitObject)
        : base(hitObject)
    {
    }

    public bool UpdateResult() => base.UpdateResult(true);

    internal void TriggerResult(bool hit)
    {
        if (AllJudged) return;

        if (hit)
            ApplyMaxResult();
        else
            ApplyMinResult();
    }
}
