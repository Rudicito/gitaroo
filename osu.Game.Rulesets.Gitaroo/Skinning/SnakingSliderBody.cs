using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Gitaroo.MathUtils;
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

    public float? AngleStart;
    public float? AngleEnd;

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

    private IHasHitObjectPath drawableSlider = null!;

    [BackgroundDependencyLoader]
    private void load(DrawableHitObject drawableObject)
    {
        drawableSlider = (IHasHitObjectPath)drawableObject;

        // Refresh();
    }

    public void UpdateProgress(double start, double end = 1)
    {
        if (drawableSlider.HitObjectPath == null)
            return;

        setRange(start, end);
    }

    public void Refresh(double start = 0, double end = 1, bool customSize = false)
    {
        SnakedStart = start;
        SnakedEnd = end;

        if (drawableSlider.HitObjectPath == null)
            return;

        // Generate the curve
        drawableSlider.HitObjectPath.GetPathToProgress(CurrentCurve, SnakedStart.Value, SnakedEnd.Value);
        SetVertices(CurrentCurve);

        // Force the body to be the final path size to avoid excessive autosize computations
        if (!customSize)
        {
            Path.AutoSizeAxes = Axes.Both;
            Size = Path.Size;

            updatePathSize();
        }

        else
        {
            Path.Size = this.customSize();
            Size = Path.Size;
        }

        snakedPosition = Path.PositionInBoundingBox(Vector2.Zero);
        snakedPathOffset = Path.PositionInBoundingBox(Path.Vertices[0]);
        snakedPathEndOffset = Path.PositionInBoundingBox(Path.Vertices[^1]);

        if (Path.Vertices.Count >= 3)
        {
            AngleStart = Angle.GetDegreesFromPosition(Path.Vertices[1], Path.Vertices[2]);
            AngleEnd = Angle.GetDegreesFromPosition(Path.Vertices[^3], Path.Vertices[^2]);
        }

        else
        {
            AngleStart = null;
            AngleEnd = null;
        }

        double lastSnakedStart = SnakedStart ?? 0;
        double lastSnakedEnd = SnakedEnd ?? 0;

        SnakedStart = null;
        SnakedEnd = null;

        setRange(lastSnakedStart, lastSnakedEnd);
    }

    private Vector2 customSize()
    {
        if (Path.Vertices.Count > 0)
        {
            float minX = float.PositiveInfinity;
            float minY = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float maxY = float.NegativeInfinity;

            foreach (var v in Path.Vertices)
            {
                minX = Math.Min(minX, v.X - PathRadius);
                minY = Math.Min(minY, v.Y - PathRadius);
                maxX = Math.Max(maxX, v.X + PathRadius);
                maxY = Math.Max(maxY, v.Y + PathRadius);
            }

            return new Vector2(maxX - minX, maxY - minY);
        }

        return Vector2.Zero;
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

        drawableSlider.HitObjectPath!.GetPathToProgress(CurrentCurve, p0, p1);

        SetVertices(CurrentCurve);
    }
}
