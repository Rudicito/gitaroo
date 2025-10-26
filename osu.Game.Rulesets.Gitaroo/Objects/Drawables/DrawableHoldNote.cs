using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Gitaroo.Judgements;
using osu.Game.Rulesets.Gitaroo.Skinning.Default;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Screens.Play;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

/// <summary>
/// Visualises a <see cref="HoldNote"/> hit object.
/// </summary>
public partial class DrawableHoldNote : DrawableTraceLineHitObject<HoldNote>, IHasSnakingSlider, IKeyBindingHandler<GitarooAction>
{
    public override bool DisplayResult => false;

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

    public IBindable<bool> IsHolding => isHolding;
    private readonly Bindable<bool> isHolding = new Bindable<bool>();

    public DrawableHeadNote Head => headContainer.Child;
    public DrawableTailNote Tail => tailContainer.Child;
    public DrawableHoldNoteBody Body => bodyContainer.Child;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRangeInternal(new Drawable[]
        {
            SliderBody = new DefaultHoldNoteBody(),
            headContainer = new Container<DrawableHeadNote>(),
            tailContainer = new Container<DrawableTailNote>(),
            bodyContainer = new Container<DrawableHoldNoteBody>(),
        });
    }

    protected override void Update()
    {
        base.Update();

        isHolding.Value = Result.IsHolding(Time.Current);

        // If user failed to track the FanShaped, stop hold state
        if (CheckFanShaped?.Invoke() == false)
        {
            Result.ReportHoldState(Time.Current, false);
        }

        UpdatePosition();
    }

    protected override JudgementResult CreateResult(Judgement judgement) => new HoldNoteJudgementResult(HitObject!, judgement);

    public new HoldNoteJudgementResult Result => (HoldNoteJudgementResult)base.Result;

    protected override void CheckForResult(bool userTriggered, double timeOffset)
    {
        if (Tail.AllJudged)
        {
            if (Tail.IsHit)
                ApplyMaxResult();
            else
                MissForcefully();

            // Make sure that the hold note is fully judged by giving the body a judgement.
            if (!Body.AllJudged)
                Body.TriggerResult(Tail.IsHit);

            // Important that this is always called when a result is applied.
            Result.ReportHoldState(Time.Current, false);
        }
    }

    public override void MissForcefully()
    {
        base.MissForcefully();

        // Important that this is always called when a result is applied.
        Result.ReportHoldState(Time.Current, false);
    }

    public bool OnPressed(KeyBindingPressEvent<GitarooAction> e)
    {
        if (AllJudged)
            return false;

        // do not run any of this logic when rewinding, as it inverts order of presses/releases.
        if ((Clock as IGameplayClock)?.IsRewinding == true)
            return false;

        if (CheckHittable?.Invoke(this, Time.Current) == false)
            return false;

        beginHoldAt(Time.Current - Head.HitObject!.StartTime);

        return Head.UpdateResult();
    }

    private void beginHoldAt(double timeOffset)
    {
        if (timeOffset < -Head.HitObject!.HitWindows.WindowFor(HitResult.Miss))
            return;

        Result.ReportHoldState(Time.Current, true);
    }

    public void OnReleased(KeyBindingReleaseEvent<GitarooAction> e)
    {
        if (AllJudged)
            return;

        // do not run any of this logic when rewinding, as it inverts order of presses/releases.
        if ((Clock as IGameplayClock)?.IsRewinding == true)
            return;

        // When our action is released and we are in the middle of a hold, there's a chance that
        // the user has released too early (before the tail).
        //
        // In such a case, we want to record this against the DrawableHoldNoteBody.
        if (isHolding.Value)
        {
            Tail.UpdateResult();
            Body.TriggerResult(Tail.IsHit);

            Result.ReportHoldState(Time.Current, false);
        }
    }

    protected virtual void UpdatePosition()
    {
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
    }

    public override void OnKilled()
    {
        base.OnKilled();
        SliderBody.RecyclePath();
    }

    private Container<DrawableHeadNote> headContainer = null!;
    private Container<DrawableTailNote> tailContainer = null!;
    private Container<DrawableHoldNoteBody> bodyContainer = null!;

    protected override void AddNestedHitObject(DrawableHitObject hitObject)
    {
        base.AddNestedHitObject(hitObject);

        switch (hitObject)
        {
            case DrawableHeadNote head:
                headContainer.Child = head;
                break;

            case DrawableTailNote tail:
                tailContainer.Child = tail;
                break;

            case DrawableHoldNoteBody body:
                bodyContainer.Child = body;
                break;
        }
    }

    protected override void ClearNestedHitObjects()
    {
        base.ClearNestedHitObjects();
        headContainer.Clear(false);
        tailContainer.Clear(false);
        bodyContainer.Clear(false);
    }

    protected override DrawableHitObject CreateNestedHitObject(HitObject hitObject)
    {
        switch (hitObject)
        {
            case HeadNote head:
                return new DrawableHeadNote(head);

            case TailNote tail:
                return new DrawableTailNote(tail);

            case HoldNoteBody body:
                return new DrawableHoldNoteBody(body);
        }

        return base.CreateNestedHitObject(hitObject);
    }

    public override void PlaySamples()
    {
        // Samples are played by the head/tail notes.
    }

    protected override void UpdateHitStateTransforms(ArmedState state)
    {
        using (BeginAbsoluteSequence(HitObject!.EndTime))
            Expire();
    }
}
