using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Layout;
using osu.Game.Rulesets.Gitaroo.Objects.Drawables;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Rulesets.UI.Scrolling.Algorithms;

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

    private readonly IBindable<double> timeRange = new BindableDouble();
    private readonly IBindable<ScrollingDirection> direction = new Bindable<ScrollingDirection>();
    private readonly IBindable<IScrollAlgorithm> algorithm = new Bindable<IScrollAlgorithm>();

    /// <summary>
    /// Whether the scrolling direction is horizontal or vertical.
    /// </summary>
    private Direction scrollingAxis => direction.Value == ScrollingDirection.Left || direction.Value == ScrollingDirection.Right ? Direction.Horizontal : Direction.Vertical;

    /// <summary>
    /// The scrolling axis is inverted if objects temporally farther in the future have a smaller position value across the scrolling axis.
    /// </summary>
    /// <example>
    /// <see cref="ScrollingDirection.Down"/> is inverted, because given two objects, one of which is at the current time and one of which is 1000ms in the future,
    /// in the current time instant the future object is spatially above the current object, and therefore has a smaller value of the Y coordinate of its position.
    /// </example>
    private bool axisInverted => direction.Value == ScrollingDirection.Down || direction.Value == ScrollingDirection.Right;

    /// <summary>
    /// A set of top-level <see cref="DrawableHitObject"/>s which have an up-to-date layout.
    /// </summary>
    private readonly HashSet<DrawableHitObject> layoutComputed = new HashSet<DrawableHitObject>();

    [Resolved]
    private IScrollingInfo scrollingInfo { get; set; } = null!;

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

        direction.ValueChanged += _ => layoutCache.Invalidate();
        timeRange.ValueChanged += _ => layoutCache.Invalidate();
        algorithm.ValueChanged += _ => layoutCache.Invalidate();
    }

    private float scrollLength => scrollingAxis == Direction.Horizontal ? DrawWidth : DrawHeight;

    public override void Add(HitObjectLifetimeEntry entry)
    {
        // Scroll info is not available until loaded.
        // The lifetime of all entries will be updated in the first Update.
        if (IsLoaded)
            setComputedLifetime(entry);

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

        foreach (var entry in Entries)
            setComputedLifetime(entry);

        algorithm.Value.Reset();

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
        RectangleF boundingBox = GetConservativeBoundingBox(entry);
        float startOffset = 0;

        switch (direction.Value)
        {
            case ScrollingDirection.Right:
                startOffset = boundingBox.Right;
                break;

            case ScrollingDirection.Down:
                startOffset = boundingBox.Bottom;
                break;

            case ScrollingDirection.Left:
                startOffset = -boundingBox.Left;
                break;

            case ScrollingDirection.Up:
                startOffset = -boundingBox.Top;
                break;
        }

        return algorithm.Value.GetDisplayStartTime(entry.HitObject.StartTime, startOffset, timeRange.Value, scrollLength);
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

        // if (hitObject.HitObject is IHasDuration e)
        // {
        //     float length = LengthAtTime(hitObject.HitObject.StartTime, e.EndTime);
        //     if (scrollingAxis == Direction.Horizontal)
        //         hitObject.Width = length;
        //     else
        //         hitObject.Height = length;
        // }

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
                traceLineHitObject.UpdatePosition();
                break;

            case DrawableTraceLine traceLine:
                updateTraceLinePosition(traceLine, Time.Current);
                break;
        }
    }

    private void updateTraceLinePosition(DrawableTraceLine traceLine, double time)
    {
        if (traceLine.HitObject == null) return;

        if (time < traceLine.HitObject.StartTime)
        {
            float lenght = algorithm.Value.GetLength(time, traceLine.HitObject.StartTime, timeRange.Value, 1);
            traceLine.UpdatePosition(0, lenght);
        }
        else
        {
            traceLine.UpdatePosition(getTraceLineProgress(traceLine, time), null);
        }
    }

    /// <summary>
    /// Computes the progress of a <see cref="DrawableTraceLine"/> along its path using the current <see cref="IScrollAlgorithm"/>.
    /// This progression value is used to calculate the position of hit objects along the trace line.
    /// </summary>
    /// <param name="traceLine">The <see cref="DrawableTraceLine"/> to calculate progression for.</param>
    /// <param name="time">The time at which to calculate progression.</param>
    /// <returns>A value between 0 and 1 representing how far along the trace line has progressed, where 0 is the start and 1 is the end.</returns>
    private double getTraceLineProgress(DrawableTraceLine traceLine, double time)
    {
        if (traceLine.HitObject == null)
            return 0;

        float traveledDistance = algorithm.Value.GetLength(traceLine.HitObject.StartTime, time, timeRange.Value, 1);

        //todo: Optimize this, always the same value for the TraceLine, so store it somewhere
        float totalDistance = algorithm.Value.GetLength(traceLine.HitObject.StartTime, traceLine.HitObject.EndTime, timeRange.Value, 1);

        if (totalDistance == 0)
            return 0;

        float progress = traveledDistance / totalDistance;

        return Math.Clamp(progress, 0f, 1f);
    }
}
