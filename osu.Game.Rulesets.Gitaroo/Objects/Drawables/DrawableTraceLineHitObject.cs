using System;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public partial class DrawableTraceLineHitObject : DrawableGitarooHitObject
{
    public DrawableTraceLineHitObject(GitarooHitObject? hitObject)
        : base(hitObject)
    {
    }

    // protected override void CheckForResult(bool userTriggered, double timeOffset)
    // {
    //     if (timeOffset >= 0)
    //         // todo: implement judgement logic
    //         ApplyMaxResult();
    // }

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
    /// Causes this <see cref="DrawableGitarooHitObject"/> to get missed, disregarding all conditions in implementations of <see cref="DrawableHitObject.CheckForResult"/>.
    /// </summary>
    public virtual void MissForcefully() => ApplyMinResult();

    /// <summary>
    /// Whether this <see cref="DrawableGitarooHitObject"/> can be hit, given a time value.
    /// If non-null, judgements will be ignored whilst the function returns false.
    /// </summary>
    public Func<DrawableHitObject, double, bool>? CheckHittable;

    public Func<double, DrawableTraceLine?>? GetTraceLine { get; set; }

    public DrawableTraceLine? TraceLine { get; set; }

    protected override void OnApply()
    {
        base.OnApply();
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
