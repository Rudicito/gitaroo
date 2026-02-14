using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public partial class DrawableHoldNoteTail : DrawableNote
{
    protected override bool UseTraceLine => false;

    protected internal DrawableHoldNote DrawableHoldNote => (DrawableHoldNote)ParentHitObject;

    public DrawableHoldNoteTail()
        : this(null)
    {
    }

    public DrawableHoldNoteTail(TailNote? hitObject)
        : base(hitObject)
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.TopLeft;
    }

    public override void UpdatePosition()
    {
        Position = OffsetPosition;
    }

    public override void UpdateOffsetPosition()
    {
        if (DrawableHoldNote.HitObject == null) return;

        OffsetPosition = DrawableHoldNote.GetPositionWithProgress(DrawableHoldNote.PathEnd!.Value);
    }

    public void UpdateResult() => base.UpdateResult(true);

    public override bool OnPressed(KeyBindingPressEvent<GitarooAction> e) => false; // Handled by the hold note

    public override void OnReleased(KeyBindingReleaseEvent<GitarooAction> e)
    {
    }
}
