using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Gitaroo.Skinning.Default;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

/// <summary>
/// Visualises a <see cref="HoldNote"/> hit object.
/// </summary>
public partial class DrawableHoldNote : DrawableTraceLineHitObject<HoldNote>, IHasSnakingSlider
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
    public double? PathStart { get; set; }

    /// <summary>
    /// The progress end of the HoldNote in the TraceLine SliderBody
    /// </summary>
    public double? PathEnd { get; set; }

    public PlaySliderBody SliderBody = null!;

    /// <summary>
    /// The Path of the related TraceLine
    /// </summary>
    public SliderPath? Path { get; set; }

    public IBindable<int> PathVersion => pathVersion;
    private readonly Bindable<int> pathVersion = new Bindable<int>();

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRangeInternal(new Drawable[]
        {
            SliderBody = new DefaultHoldNoteBody(),
            headContainer = new Container<DrawableHeadNote>()
        });
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        if (TraceLine?.HitObject == null) return;
        if (Path == null) return;

        Size = SliderBody.Size;
        Anchor = Anchor.Centre;
        Origin = Anchor.TopLeft;

        double start = Math.Clamp((Time.Current - TraceLine.HitObject.StartTime) / TraceLine.HitObject.Duration, PathStart!.Value, PathEnd!.Value);
        SliderBody.UpdateProgress(start, PathEnd!.Value);

        var pathPosition = Path.PositionAt(start);
        var positionInBoundingBox = TraceLine.SliderBody.GetPositionInBoundingBox(pathPosition);

        Position = TraceLine.Position + positionInBoundingBox - SliderBody.PathOffset;
    }

    protected override void OnApply()
    {
        base.OnApply();

        if (TraceLine?.HitObject == null) return;

        Path = TraceLine.Path;

        PathStart = (HitObject!.StartTime - TraceLine.HitObject.StartTime) / TraceLine.HitObject.Duration;
        PathEnd = (HitObject!.EndTime - TraceLine.HitObject.StartTime) / TraceLine.HitObject.Duration;

        // Ensure that the version will change after the upcoming BindTo().
        pathVersion.Value = int.MaxValue;
        PathVersion.BindTo(Path?.Version);
    }

    protected override void OnFree()
    {
        base.OnFree();

        PathVersion.UnbindFrom(Path?.Version!);

        Path = null;

        PathStart = null;
        PathEnd = null;

        SliderBody.RecyclePath();
    }

    public override void OnKilled()
    {
        base.OnKilled();
        SliderBody.RecyclePath();
    }

    private Container<DrawableHeadNote> headContainer;

    protected override void AddNestedHitObject(DrawableHitObject hitObject)
    {
        base.AddNestedHitObject(hitObject);

        switch (hitObject)
        {
            case DrawableHeadNote head:
                headContainer.Child = head;
                break;
        }
    }

    protected override void ClearNestedHitObjects()
    {
        base.ClearNestedHitObjects();
        headContainer.Clear(false);
    }

    protected override DrawableHitObject CreateNestedHitObject(HitObject hitObject)
    {
        switch (hitObject)
        {
            case HeadNote head:
                return new DrawableHeadNote(head);
        }

        return base.CreateNestedHitObject(hitObject);
    }

    protected override void UpdateHitStateTransforms(ArmedState state)
    {
        using (BeginAbsoluteSequence(HitObject!.EndTime))
            Expire();
    }
}
