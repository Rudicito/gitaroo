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
}
