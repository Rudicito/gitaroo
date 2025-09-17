using osu.Game.Rulesets.Gitaroo.Objects;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Gitaroo.Skinning.Default;

public partial class DefaultTraceLineBody : PlaySliderBody
{
    private const float path_radius = GitarooHitObject.OBJECT_RADIUS / 5;

    protected override void LoadComplete()
    {
        base.LoadComplete();

        PathRadius = path_radius;
        BorderSize = 0;
    }

    protected override Default.DrawableSliderPath CreateSliderPath() => new DrawableSliderPath();

    private partial class DrawableSliderPath : Default.DrawableSliderPath
    {
        protected override Color4 ColourAt(float position)
        {
            return Color4.Cyan;
            // return Color4.LimeGreen;
        }
    }
}
