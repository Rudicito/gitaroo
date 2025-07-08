using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Gitaroo.Skinning;
using osu.Game.Rulesets.Objects;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public partial class DrawableLineTrace : DrawableGitarooHitObject<LineTrace>, IHasHitObjectPath
{
    public DrawableLineTrace()
        : this(null)
    {
    }

    public DrawableLineTrace(LineTrace? hitObject)
        : base(hitObject)
    {
    }

    public SliderPath? HitObjectPath => HitObject?.Path;

    public SnakingSliderBody? SliderBody;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRangeInternal(new Drawable[]
        {
            SliderBody = new DefaultLineTraceBody()
        });
    }

    private partial class DefaultLineTraceBody : SnakingSliderBody
    {
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        double completionProgress = Math.Clamp((Time.Current - HitObject.StartTime) / HitObject.Duration, 0, 1);

        SliderBody?.UpdateProgress(completionProgress);
        Size = SliderBody?.Size ?? Vector2.Zero;
        OriginPosition = SliderBody?.PathOffset ?? Vector2.Zero;
    }

    protected override void OnApply()
    {
        base.OnApply();
    }

    protected override void OnFree()
    {
        base.OnFree();
    }

    public override void OnKilled()
    {
        base.OnKilled();
        SliderBody?.RecyclePath();
    }
}
