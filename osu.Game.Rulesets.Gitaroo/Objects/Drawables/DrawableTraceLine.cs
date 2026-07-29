using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Gitaroo.Skinning.Default;
using osu.Game.Rulesets.Gitaroo.UI.Scrolling;
using osu.Game.Rulesets.Gitaroo.Utils;
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

    public GitarooSliderPath? Path => HitObject?.ConvertedPath;

    public IBindable<int> PathVersion => pathVersion;
    private readonly Bindable<int> pathVersion = new Bindable<int>();

    public double? PathStart { get; set; } = 0;
    public double? PathEnd { get; set; } = 1;

    public double? SnakingStartProgress => sliderBody.SnakedStart;
    public double? SnakingEndProgress => sliderBody.SnakedEnd;

    private DefaultTraceLineBody sliderBody = null!;

    public void Refresh() => sliderBody.Refresh();

    [Resolved]
    private IGitarooScrollingInfo scrolling { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRangeInternal(new Drawable[]
        {
            sliderBody = new DefaultTraceLineBody(),
        });
    }

    public void UpdatePosition(double startProgress, double endProgress, float? length)
    {
        if (HitObject == null) return;

        Size = sliderBody.Size;
        Anchor = Anchor.Centre;
        Origin = Anchor.TopLeft;

        Vector2 offset;

        // Move the TraceLine towards the center
        if (length != null)
        {
            Direction = null;

            if (AngleStart != null)
            {
                sliderBody.UpdateProgress(0, endProgress);

                offset = -sliderBody.PathOffset;

                Position = AngleUtils.MovePoint(offset, AngleStart.Value, length.Value);
            }
        }

        // Move the TraceLine current progression to the center
        else if (startProgress < 1)
        {
            SetCurrentTraceLine!(this);

            Direction = Path!.AngleAtProgress((float)startProgress);

            sliderBody.UpdateProgress(startProgress, endProgress);

            offset = -sliderBody.PathOffset;

            Position = offset;
        }

        else
        {
            Direction = null;

            sliderBody.UpdateProgress(1);
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
        sliderBody.RecyclePath();
    }

    protected override void UpdateHitStateTransforms(ArmedState state)
    {
        using (BeginAbsoluteSequence(HitObject!.EndTime))
            Expire();
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
        var positionInBoundingBox = sliderBody.GetPositionInBoundingBox(pathPosition);

        return positionInBoundingBox;
    }

    public double GetProgressFromTime(double time) => this.GetProgressFromTime(time, scrolling);
}
