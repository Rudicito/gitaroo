using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Objects;
using osu.Game.Utils;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.Utils;

public static class SliderPathUtils
{
    public static void Scale(this SliderPath path, float scale)
    {
        for (int i = 0; i < path.ControlPoints.Count; i++)
        {
            path.ControlPoints[i].Position = GeometryUtils.GetScaledPosition(new Vector2(scale), Vector2.Zero, path.ControlPoints[i].Position);
        }
    }

    public static void GetScaledVertices(this SliderPath path, List<Vector2> finalCurve, List<(double progress, float length)> segments)
    {
        finalCurve.Clear();
        List<Vector2> curve = [];

        for (int i = 0; i < segments.Count; i++)
        {
            double startProgress = segments[i].progress;
            double endProgress = segments.Count == i + 1 ? 1 : segments[i + 1].progress;
            double rangeProgress = endProgress - startProgress;

            path.GetPathToProgress(curve, startProgress, endProgress);

            float length = (float)(rangeProgress * path.Distance);

            float curveScale = segments[i].length / length;

            Vector2 offset = finalCurve.Count == 0 ? Vector2.Zero : finalCurve[^1] - curve[0];
            Vector2 scaleOrigin = curve[0];

            for (int j = 0; j < curve.Count; j++)
            {
                curve[j] = GeometryUtils.GetScaledPosition(new Vector2(curveScale), scaleOrigin, curve[j]);
                curve[j] += offset;
            }

            // We don't add the first point when it's not the first ever curve, because it's the same as the last curve point
            finalCurve.AddRange(i == 0 ? curve : curve.Skip(1));
        }
    }
}
