using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Gitaroo.Skinning.Default;

public partial class NotePiece : CircularContainer
{
    private static Color4 borderColour = new Color4(251, 151, 75, byte.MaxValue);
    private static Color4 colour = borderColour.Opacity(0.5f);

    public Color4 AccentColor
    {
        set
        {
            colour = value;
            borderColour = value.Opacity(0.5f);
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
