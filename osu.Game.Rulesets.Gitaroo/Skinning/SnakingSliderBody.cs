using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Gitaroo.Objects.Drawables;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.Skinning;

/// <summary>
/// A <see cref="SliderBody"/> which changes its curve depending on the snaking progress.
/// </summary>
public abstract partial class SnakingSliderBody : SliderBody
{
    public readonly List<Vector2> CurrentCurve = new List<Vector2>();

    public double? SnakedStart { get; private set; }
    public double? SnakedEnd { get; private set; }

    public override float PathRadius
    {
        get => base.PathRadius;
        set
        {
            if (base.PathRadius == value)
                return;

            base.PathRadius = value;

            Refresh();
        }
    }

    /// <summary>
    /// Path Offset of the current curve
    /// </summary>
    public Vector2 PathOffset => Path.PositionInBoundingBox(Path.Vertices[0]);

    public override Vector2 PathStartOffset => snakedPathOffset;

    public override Vector2 PathEndOffset => snakedPathEndOffset;

    /// <summary>
    /// The top-left position of the path when fully snaked.
    /// </summary>
    private Vector2 snakedPosition;

    /// <summary>
    /// The offset of the path from <see cref="snakedPosition"/> when fully snaked.
    /// </summary>
    private Vector2 snakedPathOffset;

    /// <summary>
    /// The offset of the end of path from <see cref="snakedPosition"/> when fully snaked.
    /// </summary>
    private Vector2 snakedPathEndOffset;

    private IHasSnakingSlider drawableSlider = null!;

    [BackgroundDependencyLoader]
    private void load(DrawableHitObject drawableObject)
    {
        drawableSlider = (IHasSnakingSlider)drawableObject;

        // Refresh();
    }

    public void UpdateProgress(double start, double end = 1)
    {
        if (drawableSlider.Path == null)
            return;

        setRange(start, end);
    }

    public virtual void Refresh()
    {
        SnakedStart = drawableSlider.PathStart;
        SnakedEnd = drawableSlider.PathEnd;

        if (drawableSlider.Path == null)
            return;

        // Generate the curve
        drawableSlider.Path.GetPathToProgress(CurrentCurve, SnakedStart!.Value, SnakedEnd!.Value);
        SetVertices(CurrentCurve);

        // todo: Path auto-sizing calculation "acts like" there is a vertex at (0,0), causing the bounding box to be larger than expected,
        // needs fix in the osu!framework?

        // Force the body to be the final path size to avoid excessive autosize computations
        Path.AutoSizeAxes = Axes.Both;
        Size = Path.Size;

        updatePathSize();

        snakedPosition = Path.PositionInBoundingBox(Vector2.Zero);
        snakedPathOffset = Path.PositionInBoundingBox(Path.Vertices[0]);
        snakedPathEndOffset = Path.PositionInBoundingBox(Path.Vertices[^1]);

        double lastSnakedStart = SnakedStart ?? 0;
        double lastSnakedEnd = SnakedEnd ?? 0;

        SnakedStart = null;
        SnakedEnd = null;

        setRange(lastSnakedStart, lastSnakedEnd);
    }

    public override void RecyclePath()
    {
        base.RecyclePath();
        updatePathSize();
    }

    private void updatePathSize()
    {
        // Force the path to its final size to avoid excessive framebuffer resizes
        Path.AutoSizeAxes = Axes.None;
        Path.Size = Size;
    }

    private void setRange(double p0, double p1)
    {
        if (p0 > p1)
            (p0, p1) = (p1, p0);

        if (SnakedStart == p0 && SnakedEnd == p1) return;

        SnakedStart = p0;
        SnakedEnd = p1;

        drawableSlider.Path!.GetPathToProgress(CurrentCurve, p0, p1);

        SetVertices(CurrentCurve);
    }
}
