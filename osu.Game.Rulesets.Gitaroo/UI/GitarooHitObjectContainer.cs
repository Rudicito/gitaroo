using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Layout;
using osu.Framework.Lists;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Gitaroo.Objects.Drawables;
using osu.Game.Rulesets.Gitaroo.UI.Scrolling;
using osu.Game.Rulesets.Gitaroo.Utils;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Timing;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Rulesets.UI.Scrolling.Algorithms;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.UI;

public partial class GitarooHitObjectContainer : HitObjectContainer
{
    protected override int Compare(Drawable x, Drawable y)
    {
        if (!(x is DrawableHitObject xObj) || !(y is DrawableHitObject yObj))
            return base.Compare(x, y);

        // Prioritizes DrawableTraceLine
        // This way, DrawableTraceLine are always updated before DrawableTraceLineHitObject
        if (xObj is DrawableTraceLine && yObj is DrawableTraceLineHitObject)
            return -1;

        if (yObj is DrawableTraceLine && xObj is DrawableTraceLineHitObject)
            return +1;

        // Else do the base one
        return base.Compare(x, y);
    }

    /// <summary>
    /// The lenght of the TraceLine we can see, based on the beatmap AR (maybe).
    /// </summary>
    public float LenghtVisualisation = 300;

    private readonly IBindable<double> timeRange = new BindableDouble();
    private readonly IBindable<ScrollingDirection> direction = new Bindable<ScrollingDirection>();
    private readonly IBindable<IScrollAlgorithm> algorithm = new Bindable<IScrollAlgorithm>();
    private readonly IBindable<SortedList<MultiplierControlPoint>> controlPoints = new Bindable<SortedList<MultiplierControlPoint>>();

    /// <summary>
    /// A set of top-level <see cref="DrawableHitObject"/>s which have an up-to-date layout.
    /// </summary>
    private readonly HashSet<DrawableHitObject> layoutComputed = new HashSet<DrawableHitObject>();

    [Resolved]
    private IGitarooScrollingInfo scrollingInfo { get; set; } = null!;

    // Responds to changes in the layout. When the layout changes, all hit object states must be recomputed.
    private readonly LayoutValue layoutCache = new LayoutValue(Invalidation.RequiredParentSizeToFit | Invalidation.DrawInfo);

    public GitarooHitObjectContainer()
    {
        RelativeSizeAxes = Axes.Both;

        AddLayout(layoutCache);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        direction.BindTo(scrollingInfo.Direction);
        timeRange.BindTo(scrollingInfo.TimeRange);
        algorithm.BindTo(scrollingInfo.Algorithm);
        controlPoints.BindTo(scrollingInfo.ControlPoints);

        direction.ValueChanged += _ => layoutCache.Invalidate();
        timeRange.ValueChanged += _ => layoutCache.Invalidate();
        algorithm.ValueChanged += _ => layoutCache.Invalidate();
        controlPoints.ValueChanged += _ => layoutCache.Invalidate();
    }

    //todo: should be a var of the circle radius
    private float scrollLength => DrawWidth;

    public override void Add(HitObjectLifetimeEntry entry)
    {
        // Scroll info is not available until loaded.
        // The lifetime of all entries will be updated in the first Update.
        if (IsLoaded)
        {
            if (entry.HitObject is TraceLine traceLine)
                traceLine.ComputePath(scrollingInfo);

            setComputedLifetime(entry);
        }

        base.Add(entry);
    }

    protected override void AddDrawable(HitObjectLifetimeEntry entry, DrawableHitObject drawable)
    {
        base.AddDrawable(entry, drawable);

        invalidateHitObject(drawable);
        drawable.DefaultsApplied += invalidateHitObject;
    }

    protected override void RemoveDrawable(HitObjectLifetimeEntry entry, DrawableHitObject drawable)
    {
        base.RemoveDrawable(entry, drawable);

        drawable.DefaultsApplied -= invalidateHitObject;
        layoutComputed.Remove(drawable);
    }

    private void invalidateHitObject(DrawableHitObject hitObject)
    {
        layoutComputed.Remove(hitObject);
    }

    protected override void Update()
    {
        base.Update();

        if (layoutCache.IsValid) return;

        layoutComputed.Clear();

        algorithm.Value.Reset();

        foreach (var entry in Entries)
        {
            if (entry.HitObject is TraceLine traceLine)
                traceLine.ComputePath(scrollingInfo);

            setComputedLifetime(entry);
        }

        layoutCache.Validate();
    }

    protected override void UpdateAfterChildrenLife()
    {
        base.UpdateAfterChildrenLife();

        // We need to calculate hit object positions (including nested hit objects) as soon as possible after lifetimes
        // to prevent hit objects displayed in a wrong position for one frame.
        // Only AliveEntries need to be considered for layout (reduces overhead in the case of scroll speed changes).
        // We are not using AliveObjects directly to avoid selection/sorting overhead since we don't care about the order at which positions will be updated.
        foreach (var entry in AliveEntries)
        {
            var obj = entry.Value;

            updatePosition(obj, Time.Current);

            if (layoutComputed.Contains(obj))
                continue;

            updateLayoutRecursive(obj);

            layoutComputed.Add(obj);
        }
    }

    /// <summary>
    /// Get a conservative maximum bounding box of a <see cref="DrawableHitObject"/> corresponding to <paramref name="entry"/>.
    /// It is used to calculate when the hit object appears.
    /// </summary>
    protected virtual RectangleF GetConservativeBoundingBox(HitObjectLifetimeEntry entry) => new RectangleF().Inflate(100);

    private double computeDisplayStartTime(HitObjectLifetimeEntry entry)
    {
        double displayStartTime;

        switch (entry.HitObject)
        {
            case TraceLine traceLine:
                //todo: not perfect, it the traceLine do weird shapes, the TraceLine could appear too late
                displayStartTime = algorithm.Value.GetDisplayStartTime(traceLine.StartTime, GitarooHitObject.OBJECT_RADIUS + 100, timeRange.Value, scrollLength);
                break;

            case TraceLineHitObject traceLineHitObject:
                double progress = traceLineHitObject.GetProgressFromTime(traceLineHitObject.StartTime, scrollingInfo);
                double? lifetimeStartProgress = traceLineHitObject.TraceLine!.ConvertedPath.BackwardFirstCircleIntersection(progress, LenghtVisualisation);

                // The object is visible before the traceLine start
                if (lifetimeStartProgress == null)
                {
                    var startPos = traceLineHitObject.TraceLine!.ConvertedPath.PositionAt(0);
                    //todo: maybe stored AngleProgress 0 to the HitObject, because also stored in the dho
                    var directionPos = AngleUtils.MovePoint(startPos, traceLineHitObject.TraceLine.ConvertedPath.AngleAtProgress(0), 100);
                    var circlePos = traceLineHitObject.TraceLine!.ConvertedPath.PositionAt(progress);

                    var point = CircleUtils.CircleLineIntersectionSingle(startPos, directionPos, circlePos, LenghtVisualisation, startPos);
                    if (point == null)
                        throw new InvalidOperationException("Lifetime calculation error");

                    float distance = Vector2.Distance(startPos, point.Value);

                    displayStartTime = algorithm.Value.TimeAt(-distance, traceLineHitObject.TraceLine.StartTime, timeRange.Value, 1000);
                    break;
                }

                displayStartTime = traceLineHitObject.GetTimeFromProgress(lifetimeStartProgress.Value, scrollingInfo);
                break;

            default:
                displayStartTime = entry.HitObject.StartTime;
                break;
        }

        return displayStartTime;
    }

    private void setComputedLifetime(HitObjectLifetimeEntry entry)
    {
        double computedStartTime = computeDisplayStartTime(entry);

        // always load the hitobject before its first judgement offset
        entry.LifetimeStart = Math.Min(entry.HitObject.StartTime - entry.HitObject.MaximumJudgementOffset, computedStartTime);

        // This is likely not entirely correct, but sets a sane expectation of the ending lifetime.
        // A more correct lifetime will be overwritten after a DrawableHitObject is assigned via DrawableHitObject.updateState.
        //
        // It is required that we set a lifetime end here to ensure that in scenarios like loading a Player instance to a seeked
        // location in a beatmap doesn't churn every hit object into a DrawableHitObject. Even in a pooled scenario, the overhead
        // of this can be quite crippling.
        //
        // However, additionally do not attempt to alter lifetime of judged entries.
        // This is to prevent freak accidents like objects suddenly becoming alive because of this estimate assigning a later lifetime
        // than the object itself decided it should have when it underwent judgement.
        if (!entry.Judged)
            entry.LifetimeEnd = entry.HitObject.GetEndTime() + timeRange.Value;
    }

    private void updateLayoutRecursive(DrawableHitObject hitObject, double? parentHitObjectStartTime = null)
    {
        parentHitObjectStartTime ??= hitObject.HitObject.StartTime;

        if (hitObject is DrawableTraceLine traceLine)
        {
            traceLine.Refresh();
        }

        foreach (var obj in hitObject.NestedHitObjects)
        {
            updateLayoutRecursive(obj, parentHitObjectStartTime);

            // Nested hitobjects don't need to scroll, but they do need accurate positions and start lifetime
            updatePosition(obj, hitObject.HitObject.StartTime, parentHitObjectStartTime);
            setComputedLifetime(obj.Entry!);
        }
    }

    private void updatePosition(DrawableHitObject hitObject, double currentTime, double? parentHitObjectStartTime = null)
    {
        switch (hitObject)
        {
            case DrawableTraceLineHitObject traceLineHitObject:
                updateTraceLineHitObjectPosition(traceLineHitObject, currentTime);
                break;

            case DrawableTraceLine traceLine:
                updateTraceLinePosition(traceLine, currentTime);
                break;
        }
    }

    private void updateTraceLineHitObjectPosition(DrawableTraceLineHitObject traceLineHitObject, double time)
    {
        if (traceLineHitObject.TraceLine != null)
        {
            //todo: Should not be called every frame
            traceLineHitObject.UpdateOffsetPosition();

            double traceLineProgress = traceLineHitObject.TraceLine.GetProgressFromTime(time, scrollingInfo);
            traceLineHitObject.UpdateVisual(traceLineProgress);
        }

        traceLineHitObject.UpdatePosition();
    }

    private void updateTraceLinePosition(DrawableTraceLine traceLine, double time)
    {
        if (traceLine.HitObject == null) return;

        if (time < traceLine.HitObject.StartTime)
        {
            float lenght = algorithm.Value.GetLength(time, traceLine.HitObject.StartTime, timeRange.Value, 1000);
            double? endProgress = traceLine.Path!.ForwardFirstCircleIntersection(0, Math.Max(LenghtVisualisation - lenght, 0));
            traceLine.UpdatePosition(0, endProgress ?? 1, lenght);
        }
        else
        {
            double startProgress = traceLine.GetProgressFromTime(time, scrollingInfo);
            double? endProgress = traceLine.Path!.ForwardFirstCircleIntersection(startProgress, LenghtVisualisation);
            traceLine.UpdatePosition(startProgress, endProgress ?? 1, null);
        }
    }
}
