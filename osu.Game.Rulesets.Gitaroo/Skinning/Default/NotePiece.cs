using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Gitaroo.Skinning.Default;

public partial class NotePiece : CircularContainer
{
    private static readonly Color4 default_border_colour = new Color4(251, 151, 75, byte.MaxValue);
    private static readonly Color4 default_colour = default_border_colour.Opacity(0.5f);

    private Color4 borderColour = default_border_colour;
    private Color4 colour = default_colour;

    public Color4 AccentColor
    {
        set
        {
            borderColour = value;
            colour = value.Opacity(0.5f);
        }
    }

    public NotePiece()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        BorderColour = borderColour;
        BorderThickness = 2.5f;
        Masking = true;
        AddInternal(new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = colour,
        });
    }
}
