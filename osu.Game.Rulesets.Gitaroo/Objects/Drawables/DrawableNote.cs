using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Gitaroo.Skinning.Default;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

/// <summary>
/// Visualises a <see cref="Note"/> hit object.
/// </summary>
public partial class DrawableNote : DrawableTraceLineHitObject<Note>, IKeyBindingHandler<GitarooAction>
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

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRangeInternal(new Drawable[]
        {
            new MainNotePiece()
        });
    }

    protected override void Update()
    {
        base.Update();

        UpdatePosition();
    }

    protected virtual void UpdatePosition()
    {
        if (TraceLine?.HitObject == null) return;

        double traceLineProgress = Math.Clamp((HitObject!.StartTime - TraceLine.HitObject.StartTime) / TraceLine.HitObject.Duration, 0, 1);

        var pathPosition = TraceLine.Path!.PositionAt(traceLineProgress);
        var positionInBoundingBox = TraceLine.SliderBody.GetPositionInBoundingBox(pathPosition);

        Position = TraceLine.Position + positionInBoundingBox;
    }

    protected override void CheckForResult(bool userTriggered, double timeOffset)
    {
        if (!userTriggered)
        {
            if (!HitObject!.HitWindows.CanBeHit(timeOffset))
                ApplyMinResult();

            return;
        }

        var result = HitObject!.HitWindows.ResultFor(timeOffset);

        if (result == HitResult.None)
            return;

        ApplyResult(result);
    }

    public virtual bool OnPressed(KeyBindingPressEvent<GitarooAction> e)
    {
        if (e.Action != GitarooAction.LeftButton && e.Action != GitarooAction.RightButton)
            return false;

        if (CheckHittable?.Invoke(this, Time.Current) == false)
            return false;

        return UpdateResult(true);
    }

    public virtual void OnReleased(KeyBindingReleaseEvent<GitarooAction> e)
    {
    }
}
