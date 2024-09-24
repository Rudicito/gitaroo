using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Game.Rulesets.Gitaroo.UI.CenterCircle;

public partial class CenterCircleContainer : Container
{
    public CenterCircleContainer()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChildren =
        [
            new FanShaped(),
            new CenterCircle()
        ];
    }
}
