using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Gitaroo.MathUtils;
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

        if (HitObject == null) return;
        if (SliderBody == null) return;

        Size = SliderBody.Size;
        Anchor = Anchor.Centre;
        Origin = Anchor.TopLeft;

        double completionProgress = (Time.Current - HitObject.StartTime) / HitObject.Duration;

        if (completionProgress < 0)
        {
            // Move the LineTrace towards the center
            if (SliderBody.AngleStart != null)
            {
                SliderBody.UpdateProgress(0);

                Position = Angle.MovePoint(-SliderBody.PathStartOffset, SliderBody.AngleStart.Value, (float)(HitObject.Velocity * (HitObject.StartTime - Time.Current)));
            }
        }

        else
        {
            // Move the LineTrace current progression to the center
            SliderBody.UpdateProgress(Math.Clamp(completionProgress, 0, 1));
            Position = -SliderBody.PathOffset ?? Vector2.Zero;
        }
    }

    public bool IsActive => HitObject != null && Time.Current >= HitObject.StartTime && Time.Current <= HitObject.EndTime;

    public bool IsActiveAtTime(double time)
    {
        return HitObject != null && time >= HitObject.StartTime && time <= HitObject.EndTime;
    }

    protected override void OnApply()
    {
        base.OnApply();
    }

    // protected override void OnFree()
    // {
    //     base.OnFree();
    // }

    public override void OnKilled()
    {
        base.OnKilled();
        SliderBody?.RecyclePath();
    }
}
