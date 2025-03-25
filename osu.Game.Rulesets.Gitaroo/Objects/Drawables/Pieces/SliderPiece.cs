using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Lines;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables.Pieces;

public partial class SliderPiece : SmoothPath
{
    private const float border_portion = 0.4f;

    private const double fade_duration = 175;

    private const Easing fade_easing = Easing.OutQuint;

    private static readonly Color4 normal_border_colour = new Color4(202, 108, 102, byte.MaxValue);
    private static readonly Color4 normal_body_colour = normal_border_colour.Opacity(0.5f);

    private static readonly Color4 miss_border_colour = new Color4(0, 12, 106, byte.MaxValue);
    private static readonly Color4 miss_body_colour = miss_border_colour.Opacity(0.5f);

    private Color4 borderColour = normal_border_colour;

    public Color4 BorderColour
    {
        get => borderColour;
        set
        {
            if (borderColour == value)
                return;

            borderColour = value;

            InvalidateTexture();
        }
    }

    private Color4 bodyColour = normal_body_colour;

    public Color4 BodyColour
    {
        get => bodyColour;
        set
        {
            if (bodyColour == value)
                return;

            bodyColour = value;

            InvalidateTexture();
        }
    }

    protected override Color4 ColourAt(float position)
    {
        if (border_portion != 0f && position <= border_portion)
            return BorderColour;

        return BodyColour;
    }

    public void FadeToMiss()
    {
        this.TransformTo(nameof(BorderColour), miss_border_colour, fade_duration, fade_easing);
        this.TransformTo(nameof(BodyColour), miss_body_colour, fade_duration, fade_easing);
    }

    public void FadeToNormal()
    {
        this.TransformTo(nameof(borderColour), normal_border_colour, fade_duration, fade_easing);
        this.TransformTo(nameof(BodyColour), normal_body_colour, fade_duration, fade_easing);
    }

    public SliderPiece()
    {
    }
}
