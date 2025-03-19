using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables.Pieces;

public partial class HitCirclePiece : CircularContainer
{
    private static readonly Color4 border_colour = new Color4(251, 151, 75, byte.MaxValue);
    private static readonly Color4 colour = border_colour.Opacity(0.5f);

    public HitCirclePiece()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Size = new Vector2(20);
        BorderColour = border_colour;
        BorderThickness = 2.5f;
        Masking = true;
        AddInternal(new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = colour,
        });
    }
}
