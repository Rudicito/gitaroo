// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public partial class DrawableGitarooHitObject : DrawableHitObject<GitarooHitObject>
{
    public DrawableGitarooHitObject(GitarooHitObject? hitObject)
        : base(hitObject!)
    {
        // RelativeSizeAxes = Axes.X;
    }

    /// <summary>
    /// Whether this <see cref="DrawableGitarooHitObject"/> can be hit, given a time value.
    /// If non-null, judgements will be ignored whilst the function returns false.
    /// </summary>
    public Func<DrawableHitObject, double, bool>? CheckHittable;

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
}

public abstract partial class DrawableGitarooHitObject<TObject> : DrawableGitarooHitObject
    where TObject : GitarooHitObject
{
    public new TObject HitObject => (TObject)base.HitObject;

    protected DrawableGitarooHitObject(TObject? hitObject)
        : base(hitObject)
    {
    }
}
