using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Gitaroo.Skinning;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public partial class DrawableHoldNote : DrawableTraceLineHitObject<HoldNote>, IHasHitObjectPath
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
    /// The progress start of the HoldNote in the TraceLine SliderBody
    /// </summary>
    public double? ProgressStart;

    /// <summary>
    /// The progress end of the HoldNote in the TraceLine SliderBody
    /// </summary>
    public double? ProgressEnd;

    public SnakingSliderBody SliderBody = null!;

    /// <summary>
    /// The SliderPath of the related TraceLine
    /// </summary>
    public SliderPath? HitObjectPath { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRangeInternal(new Drawable[]
        {
            SliderBody = new DefaultHoldNoteBody()
        });
    }

    private partial class DefaultHoldNoteBody : SnakingSliderBody
    {
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        if (TraceLine?.HitObject == null) return;
        if (HitObjectPath == null) return;

        Size = SliderBody.Size;
        Anchor = Anchor.Centre;
        Origin = Anchor.TopLeft;

        double start = Math.Clamp((Time.Current - TraceLine.HitObject.StartTime) / TraceLine.HitObject.Duration, ProgressStart!.Value, ProgressEnd!.Value);
        SliderBody.UpdateProgress(start, ProgressEnd.Value);

        var pathPosition = HitObjectPath.PositionAt(start);
        var positionInBoundingBox = TraceLine.SliderBody.GetPositionInBoundingBox(pathPosition);

        Position = TraceLine.Position + positionInBoundingBox - SliderBody.PathOffset;
    }

    protected override void OnApply()
    {
        base.OnApply();

        if (TraceLine?.HitObject == null) return;

        HitObjectPath = TraceLine.HitObjectPath;

        ProgressStart = (HitObject!.StartTime - TraceLine.HitObject.StartTime) / TraceLine.HitObject.Duration;
        ProgressEnd = (HitObject!.EndTime - TraceLine.HitObject.StartTime) / TraceLine.HitObject.Duration;

        SliderBody.Refresh(ProgressStart.Value, ProgressEnd.Value);
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
