using osu.Framework.Bindables;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

/// <summary>
/// A DrawableHitObject that use a snaking slider
/// </summary>
public interface IHasSnakingSlider
{
    /// <summary>
    /// The path associated with the DrawableHitObject.
    /// </summary>
    SliderPath? Path { get; }

    /// <summary>
    /// The Version of the <see cref="Path"/>
    /// </summary>
    IBindable<int> PathVersion { get; }

    /// <summary>
    /// The start of the <see cref="Path"/>
    /// </summary>
    double? PathStart { get; set; }

    /// <summary>
    /// The end of the <see cref="Path"/>
    /// </summary>
    double? PathEnd { get; set; }
}
