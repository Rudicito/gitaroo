using System;
using System.Collections.Generic;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Gitaroo.Objects.Drawables;
using osu.Game.Rulesets.Gitaroo.UI.Scrolling;
using osu.Game.Rulesets.UI.Scrolling.Algorithms;

namespace osu.Game.Rulesets.Gitaroo.Utils;

public static class DrawableTraceLineUtils
{
    /// <summary>
    /// Computes the progress of a <see cref="DrawableTraceLine"/> along its path using the current <see cref="IScrollAlgorithm"/>.
    /// This progression value is used to calculate the position of hit objects along the trace line.
    /// </summary>
    /// <param name="traceLine"></param>
    /// <param name="time">The time at which to calculate progression.</param>
    /// <param name="scrollingInfo"></param>
    /// <returns>A value between 0 and 1 representing how far along the trace line has progressed, where 0 is the start and 1 is the end.</returns>
    public static double GetProgressFromTime(this DrawableTraceLine traceLine, double time, IGitarooScrollingInfo scrollingInfo)
    {
        double traceLineStartTime = traceLine.HitObject!.StartTime;
        double traceLineEndTime = traceLine.HitObject!.EndTime;
        var algorithm = scrollingInfo.Algorithm.Value;
        double timeRange = scrollingInfo.TimeRange.Value;

        traceLine.Distance ??= algorithm.GetLength(traceLineStartTime, traceLineEndTime, timeRange, 1);
        float traveledDistance = algorithm.GetLength(traceLineStartTime, time, timeRange, 1);

        if (traceLine.Distance == 0)
            return 0;

        float progress = traveledDistance / traceLine.Distance.Value;

        return Math.Clamp(progress, 0f, 1f);
    }

    public static void ComputeSegments(this DrawableTraceLine traceLine, IGitarooScrollingInfo scrollingInfo)
    {
        double traceLineStartTime = traceLine.HitObject!.StartTime;
        double traceLineEndTime = traceLine.HitObject!.EndTime;
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

            segmentLengths.Add(new(0, algorithm.GetLength(traceLineStartTime, endTime, timeRange, 1)));
        }

        for (int i = startIndex; i < controlPoints.Count; i++)
        {
            if (controlPoints[i].Time > traceLineEndTime) break;

            double startTime = segmentLengths.Count == 0 ? traceLineStartTime : controlPoints[i].Time;

            double endTime = i + 1 >= controlPoints.Count || controlPoints[i + 1].Time > traceLineEndTime
                ? traceLineEndTime
                : controlPoints[i + 1].Time;

            segmentLengths.Add(new(segmentLengths.Count == 0 ? 0 : traceLine.GetProgressFromTime(controlPoints[i].Time, scrollingInfo), algorithm.GetLength(startTime, endTime, timeRange, 1)));
        }

        if (segmentLengths.Count == 0)
            segmentLengths.Add((0, algorithm.GetLength(traceLineStartTime, traceLineEndTime, timeRange, 1)));

        traceLine.Segments = segmentLengths;
    }

    public static void ComputeDistance(this DrawableTraceLine traceLine, IGitarooScrollingInfo scrollingInfo)
    {
        double traceLineStartTime = traceLine.HitObject!.StartTime;
        double traceLineEndTime = traceLine.HitObject!.EndTime;
        var algorithm = scrollingInfo.Algorithm.Value;
        double timeRange = scrollingInfo.TimeRange.Value;

        traceLine.Distance = algorithm.GetLength(traceLineStartTime, traceLineEndTime, timeRange, 1);
    }
}
