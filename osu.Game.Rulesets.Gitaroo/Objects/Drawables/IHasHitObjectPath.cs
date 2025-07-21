using osu.Framework.Bindables;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public interface IHasHitObjectPath
{
    /// <summary>
    /// The path of the hit object associated with the drawable.
    /// </summary>
    SliderPath? HitObjectPath { get; }

    public IBindable<int> PathVersion { get; }

    public double? ProgressStart { get; set; }
    public double? ProgressEnd { get; set; }
}
