using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Bindables;
using osu.Framework.Lists;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Gitaroo.Objects.Drawables;
using osu.Game.Rulesets.Gitaroo.UI.Scrolling;
using osu.Game.Rulesets.Gitaroo.Utils;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Timing;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Rulesets.UI.Scrolling.Algorithms;

namespace osu.Game.Rulesets.Gitaroo.Tests;

public partial class TestDrawableTraceLine : TestSceneOsuGitaroo
{
    [Test]
    public void TestComputeSegments()
    {
        var segments = getLengths(1000, 2000,
            generateControlPoints(
                [(500, 0.5), (1500, 1)]
            )
        );

        Assert.That(segments.Count, Is.EqualTo(2));

        Assert.That(segments[0].progress, Is.EqualTo(0).Within(0.001));
        Assert.That(segments[1].progress, Is.EqualTo(segments[0].length / (segments[0].length + segments[1].length)).Within(0.001));

        Assert.That(segments[1].length, Is.EqualTo(segments[0].length * 2).Within(0.001));
    }

    private SortedList<MultiplierControlPoint> generateControlPoints(IReadOnlyList<(double time, double scrollSpeed)> values)
    {
        var controlPoints = new SortedList<MultiplierControlPoint>();

        foreach (var value in values)
        {
            controlPoints.Add(
                new MultiplierControlPoint(value.time)
                {
                    EffectPoint = new EffectControlPoint
                    {
                        ScrollSpeed = value.scrollSpeed
                    }
                }
            );
        }

        return controlPoints;
    }

    private List<(double progress, float length)> getLengths(double startTime, double endTime, SortedList<MultiplierControlPoint> controlPoints)
    {
        var traceLine = new TraceLine
        {
            Path = new SliderPath(),
            StartTime = startTime,
            EndTime = endTime,
            Velocity = 1
        };

        var drawableTraceLine = new DrawableTraceLine(traceLine);

        var scrollingInfo = new TestGitarooScrollingInfo
        {
            ControlPoints =
            {
                Value = controlPoints
            },
            Algorithm =
            {
                Value = new SequentialScrollAlgorithm(controlPoints)
            }
        };

        drawableTraceLine.ComputeSegments(scrollingInfo);

        return drawableTraceLine.Segments;
    }

    public class TestGitarooScrollingInfo : IGitarooScrollingInfo
    {
        public readonly Bindable<ScrollingDirection> Direction = new();
        IBindable<ScrollingDirection> IScrollingInfo.Direction => Direction;

        public readonly Bindable<double> TimeRange = new BindableDouble(1000) { Value = 1000 };
        IBindable<double> IScrollingInfo.TimeRange => TimeRange;

        public readonly Bindable<IScrollAlgorithm> Algorithm = new();
        IBindable<IScrollAlgorithm> IScrollingInfo.Algorithm => Algorithm;

        public readonly Bindable<SortedList<MultiplierControlPoint>> ControlPoints = new();
        IBindable<SortedList<MultiplierControlPoint>> IGitarooScrollingInfo.ControlPoints => ControlPoints;
    }
}
