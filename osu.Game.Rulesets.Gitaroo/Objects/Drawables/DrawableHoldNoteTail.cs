using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public partial class DrawableHoldNoteTail : DrawableNote
{
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

    protected override void UpdatePosition()
    {
        if (TraceLine?.HitObject == null) return;

        Position = DrawableHoldNote.SliderBody.PathEndOffset;
    }

    public void UpdateResult() => base.UpdateResult(true);

    public override bool OnPressed(KeyBindingPressEvent<GitarooAction> e) => false; // Handled by the hold note

    public override void OnReleased(KeyBindingReleaseEvent<GitarooAction> e)
    {
    }
}
