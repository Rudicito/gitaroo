using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Gitaroo.MathUtils;
using osu.Game.Rulesets.Gitaroo.Skinning.Default;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;

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

    public Action<DrawableTraceLine>? SetCurrentTraceLine { get; set; }

    public float? AngleStart;
    public float? AngleEnd;

    /// <summary>
    /// The current Direction of the TraceLine
    /// </summary>
    public float? Direction { get; private set; }

    public SliderPath? Path => HitObject?.Path;

    public IBindable<int> PathVersion => pathVersion;
    private readonly Bindable<int> pathVersion = new Bindable<int>();

    public double? PathStart { get; set; } = 0;
    public double? PathEnd { get; set; } = 1;

    public DefaultTraceLineBody SliderBody = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRangeInternal(new Drawable[]
        {
            SliderBody = new DefaultTraceLineBody()
        });
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        if (HitObject == null) return;

        Size = SliderBody.Size;
        Anchor = Anchor.Centre;
        Origin = Anchor.TopLeft;

        // Move the TraceLine current progression to the center
        if (Time.Current >= HitObject.StartTime && Time.Current <= HitObject.EndTime)
        {
            SetCurrentTraceLine!(this);

            double completionProgress = (Time.Current - HitObject.StartTime) / HitObject.Duration;

            Direction = Path!.AngleAtProgress((float)completionProgress);

            SliderBody.UpdateProgress(completionProgress);
            Position = -SliderBody.PathOffset;
        }

        // Move the TraceLine towards the center
        else if (Time.Current < HitObject.StartTime)
        {
            Direction = null;

            if (AngleStart != null)
            {
                SliderBody.UpdateProgress(0);

                Position = AngleUtils.MovePoint(-SliderBody.PathStartOffset, AngleStart.Value, (float)(HitObject.Velocity * (HitObject.StartTime - Time.Current)));
            }
        }

        else if (Time.Current > HitObject.EndTime)
        {
            Direction = null;

            SliderBody.UpdateProgress(1);
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

        AngleStart = Path!.AngleAtProgress(0);
        AngleEnd = Path!.AngleAtProgress(1);

        // Ensure that the version will change after the upcoming BindTo().
        pathVersion.Value = int.MaxValue;
        PathVersion.BindTo(HitObject!.Path.Version);
    }

    protected override void OnFree()
    {
        base.OnFree();

        AngleStart = null;
        AngleEnd = null;
        Direction = null;

        SliderBody.RecyclePath();

        PathVersion.UnbindFrom(Path!.Version);
    }

    public override void OnKilled()
    {
        base.OnKilled();
        SliderBody.RecyclePath();
    }

    protected override void UpdateHitStateTransforms(ArmedState state)
    {
        using (BeginAbsoluteSequence(HitObject!.EndTime))
            Expire();
    }
}
