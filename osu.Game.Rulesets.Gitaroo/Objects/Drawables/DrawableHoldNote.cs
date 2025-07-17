using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Gitaroo.Skinning;
using osu.Game.Rulesets.Objects;

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

    public SnakingSliderBody SliderBody = null!;

    /// <summary>
    /// The SliderPath of the related LineTrace
    /// </summary>
    public SliderPath? HitObjectPath { get; set; }

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

        if (LineTrace?.HitObject == null) return;
        if (HitObjectPath == null) return;

        Size = SliderBody.Size;
        Anchor = Anchor.Centre;
        Origin = Anchor.TopLeft;

        double start = Math.Clamp((Time.Current - LineTrace.HitObject.StartTime) / LineTrace.HitObject.Duration, ProgressStart!.Value, ProgressEnd!.Value);
        SliderBody.UpdateProgress(start, ProgressEnd.Value);

        var pathPosition = HitObjectPath.PositionAt(start);
        var positionInBoundingBox = LineTrace.SliderBody.GetPositionInBoundingBox(pathPosition);

        Position = LineTrace.Position + positionInBoundingBox - SliderBody.PathOffset;
    }

    protected override void OnApply()
    {
        base.OnApply();

        if (LineTrace?.HitObject == null) return;

        HitObjectPath = LineTrace.HitObjectPath;

        ProgressStart = (HitObject!.StartTime - LineTrace.HitObject.StartTime) / LineTrace.HitObject.Duration;
        ProgressEnd = (HitObject!.EndTime - LineTrace.HitObject.StartTime) / LineTrace.HitObject.Duration;

        SliderBody.Refresh(ProgressStart.Value, ProgressEnd.Value, true);
    }

    protected override void OnFree()
    {
        base.OnFree();

        HitObjectPath = null;

        ProgressStart = null;
        ProgressEnd = null;

        SliderBody.RecyclePath();
    }

    public override void OnKilled()
    {
        base.OnKilled();
        SliderBody.RecyclePath();
    }
}
