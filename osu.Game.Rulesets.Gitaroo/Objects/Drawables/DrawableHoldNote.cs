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

    /// <summary>
    /// The key that's playing the slider. Use for ignoring other input.
    /// </summary>
    public IBindable<GitarooAction?> MainKey => mainKey;

    private readonly Bindable<GitarooAction?> mainKey = new Bindable<GitarooAction?>();

    public DrawableHoldNoteHead Head => headContainer.Child;
    public DrawableHoldNoteTail Tail => tailContainer.Child;
    public DrawableHoldNoteBody Body => bodyContainer.Child;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRangeInternal(new Drawable[]
        {
            SliderBody = new DefaultHoldNoteBody(),
            headContainer = new Container<DrawableHoldNoteHead>(),
            tailContainer = new Container<DrawableHoldNoteTail>(),
            bodyContainer = new Container<DrawableHoldNoteBody>(),
        });
    }

    protected override void Update()
    {
        base.Update();

        isHolding.Value = Result.IsHolding(Time.Current);
        mainKey.Value = Result.MainKey(Time.Current);

        // If user failed to track the FanShaped, stop hold state
        if (isHolding.Value && CheckFanShaped?.Invoke() == false)
        {
            breakHold();
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
        if (isHolding.Value)
            return false;

        if (e.Action != GitarooAction.LeftButton && e.Action != GitarooAction.RightButton)
            return false;

        if (AllJudged)
            return false;

        // do not run any of this logic when rewinding, as it inverts order of presses/releases.
        if ((Clock as IGameplayClock)?.IsRewinding == true)
            return false;

        if (CheckHittable?.Invoke(this, Time.Current) == false)
            return false;

        beginHoldAt(Time.Current - Head.HitObject!.StartTime, e.Action);

        return Head.UpdateResult();
    }

    private void beginHoldAt(double timeOffset, GitarooAction? action)
    {
        if (timeOffset < -Head.HitObject!.HitWindows.WindowFor(HitResult.Miss))
            return;

        Result.ReportHoldState(Time.Current, true, action);
    }

    public void OnReleased(KeyBindingReleaseEvent<GitarooAction> e)
    {
        if (mainKey.Value != e.Action)
            return;

        if (AllJudged)
            return;

        // do not run any of this logic when rewinding, as it inverts order of presses/releases.
        if ((Clock as IGameplayClock)?.IsRewinding == true)
            return;

        if (isHolding.Value)
        {
            breakHold();
        }
    }

    private void breakHold()
    {
        Tail.UpdateResult();
        Body.TriggerResult(Tail.IsHit);

        Result.ReportHoldState(Time.Current, false);
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

    private Container<DrawableHoldNoteHead> headContainer = null!;
    private Container<DrawableHoldNoteTail> tailContainer = null!;
    private Container<DrawableHoldNoteBody> bodyContainer = null!;

    protected override void AddNestedHitObject(DrawableHitObject hitObject)
    {
        base.AddNestedHitObject(hitObject);

        switch (hitObject)
        {
            case DrawableHoldNoteHead head:
                headContainer.Child = head;
                break;

            case DrawableHoldNoteTail tail:
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
                return new DrawableHoldNoteHead(head);

            case TailNote tail:
                return new DrawableHoldNoteTail(tail);

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
        if (AllJudged)
            Expire();
    }
}
