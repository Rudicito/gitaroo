using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Rulesets.Gitaroo.Objects.Drawables;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Skinning;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Gitaroo.Skinning.Default;

public partial class PlaySliderBody : SnakingSliderBody
{
    protected IBindable<Color4> AccentColourBindable { get; private set; } = null!;

    private IBindable<int> pathVersion = null!;

    [BackgroundDependencyLoader]
    private void load(ISkinSource skin, DrawableHitObject drawableObject)
    {
        AccentColourBindable = drawableObject.AccentColour.GetBoundCopy();
        AccentColourBindable.BindValueChanged(accent => AccentColour = GetBodyAccentColour(skin, accent.NewValue), true);

        if (drawableObject is IHasHitObjectPath drawableSlider)
        {
            pathVersion = drawableSlider.PathVersion.GetBoundCopy();
            pathVersion.BindValueChanged(_ => Scheduler.AddOnce(Refresh));
        }

        BorderColour = GetBorderColour(skin);
    }

    protected virtual Color4 GetBorderColour(ISkinSource skin) => Color4.White;

    protected virtual Color4 GetBodyAccentColour(ISkinSource skin, Color4 hitObjectAccentColour) => hitObjectAccentColour;
}
