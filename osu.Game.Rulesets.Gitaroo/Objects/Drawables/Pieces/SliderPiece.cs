using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics.Lines;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables.Pieces;

public partial class SliderPiece : SmoothPath
{
    private const float border_portion = 0.3f;

    private static readonly Color4 border_colour = new Color4(202, 108, 102, byte.MaxValue);
    private static readonly Color4 colour = border_colour.Opacity(0.5f);

    protected override Color4 ColourAt(float position)
    {
        if (border_portion != 0f && position <= border_portion)
            return border_colour;

        return colour;
    }

    public SliderPiece()
    {
    }
}
