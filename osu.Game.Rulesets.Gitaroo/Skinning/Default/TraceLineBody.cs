using osu.Game.Rulesets.Gitaroo.MathUtils;

namespace osu.Game.Rulesets.Gitaroo.Skinning.Default;

public partial class TraceLineBody : PlaySliderBody
{
    public float? AngleStart;
    public float? AngleEnd;

    public override void Refresh()
    {
        base.Refresh();

        AngleStart = null;
        AngleEnd = null;

        int vertexCount = Path.Vertices.Count;

        if (vertexCount >= 2)
        {
            for (int i = 0; i < vertexCount - 1; i++)
            {
                if (Path.Vertices[i] == Path.Vertices[i + 1]) continue;

                AngleStart = Angle.GetDegreesFromPosition(Path.Vertices[i], Path.Vertices[i + 1]);
                break;
            }

            for (int i = vertexCount - 1; i > 0; i--)
            {
                if (Path.Vertices[i - 1] == Path.Vertices[i]) continue;

                AngleEnd = Angle.GetDegreesFromPosition(Path.Vertices[i - 1], Path.Vertices[i]);
                break;
            }
        }
    }
}
