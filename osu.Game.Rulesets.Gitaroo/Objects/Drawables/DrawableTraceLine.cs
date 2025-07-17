using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Gitaroo.MathUtils;
using osu.Game.Rulesets.Gitaroo.Skinning;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public partial class DrawableTraceLine : DrawableGitarooHitObject<TraceLine>, IHasHitObjectPath
{
    public DrawableTraceLine()
        : this(null)
    {
    }

    public DrawableTraceLine(TraceLine? hitObject)
        : base(hitObject)
    {
    }

    public SliderPath? HitObjectPath => HitObject?.Path;

    public SnakingSliderBody SliderBody = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRangeInternal(new Drawable[]
        {
            SliderBody = new DefaultTraceLineBody()
        });
    }

    private partial class DefaultTraceLineBody : SnakingSliderBody
    {
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        if (HitObject == null) return;

        Size = SliderBody.Size;
        Anchor = Anchor.Centre;
        Origin = Anchor.TopLeft;

        if (Time.Current >= HitObject.StartTime)
        {
            // Move the TraceLine current progression to the center
            double completionProgress = (Time.Current - HitObject.StartTime) / HitObject.Duration;
            SliderBody.UpdateProgress(Math.Clamp(completionProgress, 0, 1));
            Position = -SliderBody.PathOffset;
        }

        else
        {
            // Move the TraceLine towards the center
            if (SliderBody.AngleStart != null)
            {
                SliderBody.UpdateProgress(0);

                Position = Angle.MovePoint(-SliderBody.PathStartOffset, SliderBody.AngleStart.Value, (float)(HitObject.Velocity * (HitObject.StartTime - Time.Current)));
            }
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

        SliderBody.Refresh();
    }

    protected override void OnFree()
    {
        base.OnFree();
        SliderBody.RecyclePath();
    }

    public override void OnKilled()
    {
        base.OnKilled();
        SliderBody.RecyclePath();
    }
}
