namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public partial class DrawableTailNote : DrawableTraceLineHitObject<TailNote>
{
    public override bool DisplayResult => false;

    protected internal DrawableHoldNote HoldNote => (DrawableHoldNote)ParentHitObject;

    public DrawableTailNote()
        : this(null)
    {
    }

    public DrawableTailNote(TailNote? hitObject)
        : base(hitObject)
    {
    }

    public void UpdateResult() => base.UpdateResult(true);

    protected override void CheckForResult(bool userTriggered, double timeOffset)
    {
        // TODO: No sample when not releasing because user triggered always check!

        if (!userTriggered) return;

        if (HoldNote.IsHolding.Value && timeOffset >= -TailNote.RELEASE_WINDOW_LENIENCE)
            ApplyMaxResult();

        else
            ApplyMinResult();
    }
}
