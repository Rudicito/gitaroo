using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables.Pieces;

public partial class HitCirclePiece : CircularContainer
{
    public HitCirclePiece()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Size = new Vector2(20);
        BorderColour = new Color4(251, 151, 75, byte.MaxValue);
        BorderThickness = 2.5f;
        Masking = true;
        AddInternal(new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = new Color4(251, 151, 75, 128),
        });
    }
}
