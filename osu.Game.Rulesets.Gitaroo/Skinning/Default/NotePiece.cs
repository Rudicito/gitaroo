using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Gitaroo.Skinning.Default;

/// <remarks>
/// Uses <see cref="CircularContainer"/> with an internal <see cref="Box"/> instead of <see cref="Circle"/>
/// to enable independent control of border and fill colors. With <see cref="Circle"/>, opacity changes
/// affect both border and fill simultaneously. This approach allows separate control via the container's
/// BorderColour and the inner box's Colour property.
/// </remarks>
public partial class NotePiece : CircularContainer
{
    private static readonly Color4 default_border_colour = new Color4(251, 151, 75, byte.MaxValue);
    private static readonly Color4 default_colour = default_border_colour.Opacity(0.5f);

    /// <summary>
    /// The inner box that gives the fill color of the note.
    /// </summary>
    private readonly Box innerBox;

    /// <summary>
    /// Sets the border and fill color of the note.
    /// </summary>
    public Color4 AccentColor
    {
        set
        {
            BorderColour = value;
            innerBox.Colour = value.Opacity(0.5f);
        }
    }

    public NotePiece()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        BorderColour = default_border_colour;
        BorderThickness = 2.5f;
        Masking = true;
        AddInternal(innerBox = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = default_colour,
        });
    }
}
