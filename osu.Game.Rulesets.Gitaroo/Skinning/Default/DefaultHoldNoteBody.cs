using osu.Framework.Extensions.Color4Extensions;
using osu.Game.Rulesets.Gitaroo.Objects;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Gitaroo.Skinning.Default;

public partial class DefaultHoldNoteBody : PlaySliderBody
{
    private static readonly Color4 miss_border_colour = new Color4(0, 12, 106, byte.MaxValue);
    private static readonly Color4 miss_body_colour = miss_border_colour.Opacity(0.5f);

    protected override void LoadComplete()
    {
        const float path_radius = GitarooHitObject.OBJECT_RADIUS;

        base.LoadComplete();

        AccentColourBindable.BindValueChanged(accent => BorderColour = accent.NewValue, true);

        PathRadius = path_radius;
        BorderSize = path_radius / 4;
    }

    protected override Default.DrawableSliderPath CreateSliderPath() => new DrawableSliderPath();

    private partial class DrawableSliderPath : Default.DrawableSliderPath
    {
        private const float border_portion = 0.4f;

        protected override Color4 ColourAt(float position)
        {
            if (CalculatedBorderPortion != 0f && position <= CalculatedBorderPortion)
                return BorderColour;

            return AccentColour.Opacity(0.5f);
        }

        // protected override Color4 ColourAt(float position)
        // {
        //     if (CalculatedBorderPortion != 0f && position <= CalculatedBorderPortion)
        //         return Color4.Red;
        //
        //     return Color4.Green;
        // }
    }
}
