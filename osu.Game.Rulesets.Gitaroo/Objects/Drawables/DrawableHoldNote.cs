using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Gitaroo.Skinning;
using osu.Game.Rulesets.Objects;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public partial class DrawableHoldNote : DrawableLineTraceHitObject<HoldNote>, IHasHitObjectPath
{
    public DrawableHoldNote()
        : this(null)
    {
    }

    public DrawableHoldNote(HoldNote? hitObject)
        : base(hitObject)
    {
    }

    /// <summary>
    /// The progress start of the HoldNote in the LineTrace SliderBody
    /// </summary>
    public double? ProgressStart;

    /// <summary>
    /// The progress end of the HoldNote in the LineTrace SliderBody
    /// </summary>
    public double? ProgressEnd;

    public SnakingSliderBody? SliderBody;

    public SliderPath? HitObjectPath { get; set; } = null;

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
        if (LineTrace?.HitObject == null) return;
        if (HitObjectPath == null) return;
        if (ProgressStart == null || ProgressEnd == null) return;

        Size = SliderBody.Size;
        Anchor = Anchor.Centre;
        Origin = Anchor.TopLeft;

        if (Time.Current >= HitObject.StartTime)
        {
            double start = (Time.Current - LineTrace.HitObject.StartTime) / LineTrace.HitObject.Duration;
            // (Time.Current - HitObject.StartTime) / HitObject.Duration

            SliderBody.UpdateProgress(start, ProgressEnd.Value);
            Position = -SliderBody.PathOffset ?? Vector2.Zero;
        }

        else
        {
            SliderBody.UpdateProgress(ProgressStart.Value, ProgressEnd.Value);
            var result = HitObjectPath.PositionAt(ProgressStart.Value);
            Position = LineTrace.Position;
        }

        // Position = -SliderBody.PathOffset ?? Vector2.Zero;
    }

    protected override void OnApply()
    {
        base.OnApply();

        if (LineTrace?.HitObject == null) return;

        HitObjectPath = LineTrace.HitObjectPath;

        ProgressStart = (HitObject!.StartTime - LineTrace.HitObject.StartTime) / LineTrace.HitObject.Duration;
        ProgressEnd = (HitObject!.EndTime - LineTrace.HitObject.StartTime) / LineTrace.HitObject.Duration;

        SliderBody?.Refresh(ProgressStart.Value, ProgressEnd.Value);
    }

    protected override void OnFree()
    {
        base.OnFree();

        HitObjectPath = null;

        ProgressStart = null;
        ProgressEnd = null;

        SliderBody?.RecyclePath();
    }

    public override void OnKilled()
    {
        base.OnKilled();
        SliderBody?.RecyclePath();
    }
}
