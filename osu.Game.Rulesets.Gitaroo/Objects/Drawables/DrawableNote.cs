using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Gitaroo.Skinning.Default;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

/// <summary>
/// Visualises a <see cref="Note"/> hit object.
/// </summary>
public partial class DrawableNote : DrawableTraceLineHitObject<Note>
{
    public DrawableNote()
        : this(null)
    {
    }

    public DrawableNote(Note? hitObject)
        : base(hitObject)
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.TopLeft;
    }

    private MainNotePiece mainNotePiece = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRangeInternal(new Drawable[]
        {
            mainNotePiece = new MainNotePiece()
        });
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        if (TraceLine?.HitObject == null) return;

        double traceLineProgress = Math.Clamp((HitObject!.StartTime - TraceLine.HitObject.StartTime) / TraceLine.HitObject.Duration, 0, 1);

        var pathPosition = TraceLine.Path!.PositionAt(traceLineProgress);
        var positionInBoundingBox = TraceLine.SliderBody.GetPositionInBoundingBox(pathPosition);

        Position = TraceLine.Position + positionInBoundingBox;
    }
}
