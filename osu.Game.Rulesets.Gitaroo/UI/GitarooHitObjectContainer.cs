using osu.Framework.Graphics;
using osu.Game.Rulesets.Gitaroo.Objects.Drawables;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Gitaroo.UI;

public partial class GitarooHitObjectContainer : HitObjectContainer
{
    public GitarooHitObjectContainer()
    {
        RelativeSizeAxes = Axes.Both;
    }

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
}
