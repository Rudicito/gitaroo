using System;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

/// <summary>
/// A DrawableHitObject who is in set in a <see cref="TraceLine"/>
/// </summary>
public partial class DrawableTraceLineHitObject : DrawableGitarooHitObject
{
    public DrawableTraceLineHitObject(GitarooHitObject? hitObject)
        : base(hitObject)
    {
    }

    protected override void UpdateHitStateTransforms(ArmedState state)
    {
        switch (state)
        {
            case ArmedState.Miss:
                this.FadeOut(150, Easing.In);
                break;

            case ArmedState.Hit:
                this.FadeOut();
                break;
        }
    }

    /// <summary>
    /// Causes this <see cref="DrawableGitarooHitObject"/> to get missed, disregarding all conditions in implementations of DrawableHitObject.CheckForResult.
    /// </summary>
    public virtual void MissForcefully() => ApplyMinResult();

    /// <summary>
    /// Whether this <see cref="DrawableGitarooHitObject"/> can be hit, given a time value.
    /// If non-null, judgements will be ignored whilst the function returns false.
    /// </summary>
    public Func<DrawableHitObject, double, bool>? CheckHittable;

    /// <summary>
    /// Whether the FanShaped is being tracked.
    /// </summary>
    public Func<bool>? CheckFanShaped;

    public Vector2 OffsetPosition { get; set; } = Vector2.Zero;

    public virtual void UpdatePosition()
    {
        if (TraceLine == null) return;

        Position = TraceLine.Position + OffsetPosition;
    }

    public virtual void UpdateOffsetPosition()
    {
    }

    protected override void Update()
    {
        base.Update();

        //todo: Should not be called at every frames for better performance
        // (should be called after Refresh() of SnakingSlider for example, or called with a bindable)
        UpdateOffsetPosition();

        UpdatePosition();
    }

    /// <summary>
    /// A bool use to use or not the TraceLine.
    /// Set to false for example the <see cref="DrawableHoldNoteHead"/> and <see cref="DrawableHoldNoteTail"/>
    /// </summary>
    protected virtual bool UseTraceLine => true;

    public Func<double, DrawableTraceLine?>? GetTraceLine { get; set; }

    public DrawableTraceLine? TraceLine { get; set; }

    protected override void OnApply()
    {
        base.OnApply();
        if (UseTraceLine)
            TraceLine = GetTraceLine!(HitObject!.StartTime);
    }

    protected override void OnFree()
    {
        base.OnFree();
        TraceLine = null;
    }
}

public abstract partial class DrawableTraceLineHitObject<TObject> : DrawableTraceLineHitObject
    where TObject : GitarooHitObject
{
    public new TObject? HitObject => (TObject?)base.HitObject;

    protected DrawableTraceLineHitObject(TObject? hitObject)
        : base(hitObject)
    {
    }
}
