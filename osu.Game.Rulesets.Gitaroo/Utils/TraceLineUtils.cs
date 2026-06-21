using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Gitaroo.Objects.Drawables;
using osu.Game.Rulesets.Gitaroo.UI.Scrolling;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.UI.Scrolling.Algorithms;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.Utils;

public static class TraceLineUtils
{
    /// <inheritdoc cref="GetProgressFromTime(TraceLine, double, IGitarooScrollingInfo)"/>
    public static double GetProgressFromTime(this DrawableTraceLine drawableTraceLine, double time, IGitarooScrollingInfo scrollingInfo)
    {
        return drawableTraceLine.HitObject!.GetProgressFromTime(time, scrollingInfo);
    }

    /// <inheritdoc cref="GetProgressFromTime(TraceLine, double, IGitarooScrollingInfo)"/>
    public static double GetProgressFromTime(this TraceLineHitObject traceLineHitObject, double time, IGitarooScrollingInfo scrollingInfo)
        => traceLineHitObject.TraceLine?.GetProgressFromTime(time, scrollingInfo) ?? 0;

    /// <summary>
    /// Computes the progress of a <see cref="TraceLine"/> along its path using the current <see cref="IScrollAlgorithm"/>.
    /// This progression value is used to calculate the position of hit objects along the trace line.
    /// </summary>
    /// <returns>A value between 0 and 1 representing how far along the trace line has progressed, where 0 is the start and 1 is the end.</returns>
    public static double GetProgressFromTime(this TraceLine traceLine, double time, IGitarooScrollingInfo scrollingInfo)
    {
        double traceLineStartTime = traceLine.StartTime;
        double distance = traceLine.ConvertedDistance;
        var algorithm = scrollingInfo.Algorithm.Value;
        double timeRange = scrollingInfo.TimeRange.Value;

        float traveledDistance = algorithm.GetLength(traceLineStartTime, time, timeRange, 1000);

        if (distance == 0)
            return 0;

        double progress = traveledDistance / distance;

        return Math.Clamp(progress, 0d, 1d);
    }

    public static double GetTimeFromProgress(this DrawableTraceLine drawableTraceLine, double progress, IGitarooScrollingInfo scrollingInfo)
    {
        return drawableTraceLine.HitObject!.GetTimeFromProgress(progress, scrollingInfo);
    }

    public static double GetTimeFromProgress(this TraceLineHitObject traceLineHitObject, double progress, IGitarooScrollingInfo scrollingInfo)
        => traceLineHitObject.TraceLine?.GetTimeFromProgress(progress, scrollingInfo) ?? 0;

    public static double GetTimeFromProgress(this TraceLine traceLine, double progress, IGitarooScrollingInfo scrollingInfo)
    {
        double distance = traceLine.ConvertedDistance;
        var algorithm = scrollingInfo.Algorithm.Value;
        double timeRange = scrollingInfo.TimeRange.Value;

        float targetPosition = (float)(progress * distance);
        return algorithm.TimeAt(targetPosition, traceLine.StartTime, timeRange, 1000);
    }

    public static List<(double progress, float length)> ComputeSegments(this TraceLine traceLine, IGitarooScrollingInfo scrollingInfo)
    {
        double traceLineStartTime = traceLine.StartTime;
        double traceLineEndTime = traceLine.EndTime;
        var controlPoints = scrollingInfo.ControlPoints.Value;
        var algorithm = scrollingInfo.Algorithm.Value;
        double timeRange = scrollingInfo.TimeRange.Value;

        List<(double progress, float length)> segmentLengths = [];

        int startIndex = ControlPointInfo.BinarySearch(controlPoints, traceLineStartTime, EqualitySelection.Rightmost);

        if (startIndex < 0)
            startIndex = ~startIndex - 1;

        if (startIndex < 0)
        {
            startIndex = 0;

            double endTime = controlPoints.Count == 0 ? traceLineEndTime : controlPoints[0].Time;

            segmentLengths.Add(new(0, algorithm.GetLength(traceLineStartTime, endTime, timeRange, 1000)));
        }

        for (int i = startIndex; i < controlPoints.Count; i++)
        {
            if (controlPoints[i].Time > traceLineEndTime) break;

            double startTime = segmentLengths.Count == 0 ? traceLineStartTime : controlPoints[i].Time;

            double endTime = i + 1 >= controlPoints.Count || controlPoints[i + 1].Time > traceLineEndTime
                ? traceLineEndTime
                : controlPoints[i + 1].Time;

            segmentLengths.Add(new(segmentLengths.Count == 0 ? 0 : traceLine.GetProgressFromTime(controlPoints[i].Time, scrollingInfo), algorithm.GetLength(startTime, endTime, timeRange, 1000)));
        }

        if (segmentLengths.Count == 0)
            segmentLengths.Add((0, algorithm.GetLength(traceLineStartTime, traceLineEndTime, timeRange, 1000)));

        return segmentLengths;
    }

    public static void ComputePath(this TraceLine traceLine, IGitarooScrollingInfo scrollingInfo)
    {
        traceLine.ConvertedPath.ControlPoints.Clear();

        // Generate the curve
        List<Vector2> fullCurve = [];
        var segments = traceLine.ComputeSegments(scrollingInfo);
        if (segments.Count == 0) return;

        traceLine.Path.GetScaledVertices(fullCurve, segments);

        var pathControlPoints = fullCurve.Select(c => new PathControlPoint(c, PathType.LINEAR)).ToArray();
        traceLine.ConvertedPath.ControlPoints.AddRange(pathControlPoints);
    }
}
