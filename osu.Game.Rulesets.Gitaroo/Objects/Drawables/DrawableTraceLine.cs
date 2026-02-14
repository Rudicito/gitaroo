using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Gitaroo.Skinning.Default;
using osu.Game.Rulesets.Gitaroo.Utils;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;

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
            SliderBody = new DefaultTraceLineBody(),
        });
    }

    protected override void Update()
    {
        base.Update();

        UpdatePosition();
    }

    protected void UpdatePosition()
    {
        if (HitObject == null) return;

        Size = SliderBody.Size;
        Anchor = Anchor.Centre;
        Origin = Anchor.TopLeft;

        Vector2 offset;

        // Move the TraceLine current progression to the center
        if (Time.Current >= HitObject.StartTime && Time.Current <= HitObject.EndTime)
        {
            SetCurrentTraceLine!(this);

            double completionProgress = (Time.Current - HitObject.StartTime) / HitObject.Duration;

            Direction = Path!.AngleAtProgress((float)completionProgress);

            SliderBody.UpdateProgress(completionProgress);

            offset = -SliderBody.PathOffset;

            Position = offset;
        }

        // Move the TraceLine towards the center
        else if (Time.Current < HitObject.StartTime)
        {
            Direction = null;

            if (AngleStart != null)
            {
                SliderBody.UpdateProgress(0);

                offset = -SliderBody.PathOffset;

                Position = AngleUtils.MovePoint(offset, AngleStart.Value, (float)(HitObject.Velocity * (HitObject.StartTime - Time.Current)));
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

    /// <summary>
    /// Get the position along the path of the <see cref="DrawableTraceLine"/> at a specific point in time.
    /// </summary>
    /// <param name="time">The time at which to calculate the position.</param>
    /// <param name="minProgress">The minimum progress value to clamp to (default is 0).</param>
    /// <param name="maxProgress">The maximum progress value to clamp to (default is 1).</param>
    /// <returns>The position along the path of the <see cref="DrawableTraceLine"/>.</returns>
    public Vector2 GetPositionWithTime(double time, double minProgress = 0, double maxProgress = 1)
    {
        if (HitObject == null) return Vector2.Zero;

        double traceLineProgress = GetProgressWithTime(time, minProgress, maxProgress);

        return GetPositionWithProgress(traceLineProgress);
    }

    /// <summary>
    /// Get the progress of the <see cref="DrawableTraceLine"/> at a specific point in time.
    /// </summary>
    /// <param name="time">The time at which to calculate the progress.</param>
    /// <param name="minProgress">The minimum progress value to clamp to (default is 0).</param>
    /// <param name="maxProgress">The maximum progress value to clamp to (default is 1).</param>
    /// <returns>The progress of the <see cref="DrawableTraceLine"/>, by default between 0 and 1.</returns>
    public double GetProgressWithTime(double time, double minProgress = 0, double maxProgress = 1)
    {
        if (HitObject == null) return 0;

        return Math.Clamp((time - HitObject.StartTime) / HitObject.Duration, minProgress, maxProgress);
    }

    /// <summary>
    /// Get the position along the path of the <see cref="DrawableTraceLine"/> with the specific progress.
    /// </summary>
    /// <param name="progress">The progress value used to get the position</param>
    /// <returns>The position along the path of the <see cref="DrawableTraceLine"/></returns>
    public Vector2 GetPositionWithProgress(double progress)
    {
        if (HitObject == null) return Vector2.Zero;

        var pathPosition = Path!.PositionAt(progress);
        var positionInBoundingBox = SliderBody.GetPositionInBoundingBox(pathPosition);

        return positionInBoundingBox;
    }
}
