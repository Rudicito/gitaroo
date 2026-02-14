using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public partial class DrawableHoldNoteHead : DrawableNote
{
    protected override bool UseTraceLine => false;

    public DrawableHoldNote DrawableHoldNote => (DrawableHoldNote)ParentHitObject;

    public DrawableHoldNoteHead()
        : this(null)
    {
    }

    public DrawableHoldNoteHead(HeadNote? hitObject)
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

        OffsetPosition = DrawableHoldNote.GetPositionWithProgress(DrawableHoldNote.PathStart!.Value);
    }

    public bool UpdateResult() => base.UpdateResult(true);

    public override bool OnPressed(KeyBindingPressEvent<GitarooAction> e) => false; // Handled by the hold note

    public override void OnReleased(KeyBindingReleaseEvent<GitarooAction> e)
    {
    }
}
