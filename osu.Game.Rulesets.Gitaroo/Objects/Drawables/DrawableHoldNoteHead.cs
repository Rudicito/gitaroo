using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public partial class DrawableHoldNoteHead : DrawableNote
{
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

    protected override void UpdatePosition()
    {
        if (TraceLine?.HitObject == null) return;

        Position = DrawableHoldNote.SliderBody.PathOffset;
    }

    public bool UpdateResult() => base.UpdateResult(true);

    public override bool OnPressed(KeyBindingPressEvent<GitarooAction> e) => false; // Handled by the hold note

    public override void OnReleased(KeyBindingReleaseEvent<GitarooAction> e)
    {
    }
}
