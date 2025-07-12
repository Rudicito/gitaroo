using System;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public partial class DrawableLineTraceHitObject : DrawableGitarooHitObject
{
    public DrawableLineTraceHitObject(GitarooHitObject? hitObject)
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

    public Func<double, DrawableLineTrace?>? GetCurrentLineTrace { get; set; }

    public DrawableLineTrace? CurrentLineTrace { get; set; }

    protected override void OnApply()
    {
        base.OnApply();
        CurrentLineTrace = GetCurrentLineTrace!(HitObject!.StartTime);
    }

    protected override void OnFree()
    {
        base.OnFree();
        CurrentLineTrace = null;
    }
}

public abstract partial class DrawableLineTraceHitObject<TObject> : DrawableLineTraceHitObject
    where TObject : GitarooHitObject
{
    public new TObject? HitObject => (TObject?)base.HitObject;

    protected DrawableLineTraceHitObject(TObject? hitObject)
        : base(hitObject)
    {
    }
}
