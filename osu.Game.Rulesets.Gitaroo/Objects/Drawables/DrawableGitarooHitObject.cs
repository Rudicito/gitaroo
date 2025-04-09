// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public partial class DrawableGitarooHitObject : DrawableHitObject<GitarooHitObject>
{
    public DrawableGitarooHitObject(GitarooHitObject hitObject)
        : base(hitObject)
    {
        // RelativeSizeAxes = Axes.X;
    }

    protected override void CheckForResult(bool userTriggered, double timeOffset)
    {
        if (timeOffset >= 0)
            // todo: implement judgement logic
            ApplyMaxResult();
    }

    protected override void UpdateHitStateTransforms(ArmedState state)
    {
        const double duration = 1000;

        switch (state)
        {
            case ArmedState.Hit:
                this.FadeOut(duration, Easing.OutQuint).Expire();
                break;

            case ArmedState.Miss:

                this.FadeColour(Color4.Red, duration);
                this.FadeOut(duration, Easing.InQuint).Expire();
                break;
        }
    }
}

public abstract partial class DrawableGitarooHitObject<TObject> : DrawableGitarooHitObject
    where TObject : GitarooHitObject
{
    public new TObject HitObject => (TObject)base.HitObject;

    protected DrawableGitarooHitObject(TObject hitObject)
        : base(hitObject)
    {
    }
}
