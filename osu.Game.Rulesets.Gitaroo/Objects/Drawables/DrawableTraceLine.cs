using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Gitaroo.MathUtils;
using osu.Game.Rulesets.Gitaroo.Skinning.Default;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

/// <summary>
/// Visualises a <see cref="TraceLine"/> hit object.
/// </summary>
public partial class DrawableTraceLine : DrawableGitarooHitObject<TraceLine>, IHasSnakingSlider
{
    public DrawableTraceLine()
        : this(null)
    {
    }

    public DrawableTraceLine(TraceLine? hitObject)
        : base(hitObject)
    {
    }

    public SliderPath? Path => HitObject?.Path;

    public IBindable<int> PathVersion => pathVersion;
    private readonly Bindable<int> pathVersion = new Bindable<int>();

    public double? PathStart { get; set; } = 0;
    public double? PathEnd { get; set; } = 1;

    public TraceLineBody SliderBody = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRangeInternal(new Drawable[]
        {
            SliderBody = new DefaultTraceLineBody()
        });
    }

    private partial class DefaultTraceLineBody : TraceLineBody
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

        // Ensure that the version will change after the upcoming BindTo().
        pathVersion.Value = int.MaxValue;
        PathVersion.BindTo(HitObject!.Path.Version);
    }

    protected override void OnFree()
    {
        base.OnFree();
        SliderBody.RecyclePath();

        PathVersion.UnbindFrom(Path!.Version);
    }

    public override void OnKilled()
    {
        base.OnKilled();
        SliderBody.RecyclePath();
    }
}
