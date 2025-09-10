// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public partial class DrawableGitarooHitObject : DrawableHitObject<GitarooHitObject>
{
    public DrawableGitarooHitObject(GitarooHitObject? hitObject)
        : base(hitObject!)
    {
        // RelativeSizeAxes = Axes.X;
    }
}

public abstract partial class DrawableGitarooHitObject<TObject> : DrawableGitarooHitObject
    where TObject : GitarooHitObject
{
    public new TObject? HitObject => (TObject?)base.HitObject;

    protected DrawableGitarooHitObject(TObject? hitObject)
        : base(hitObject)
    {
    }
}
