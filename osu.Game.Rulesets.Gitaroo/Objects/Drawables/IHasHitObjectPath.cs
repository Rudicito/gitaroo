using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public interface IHasHitObjectPath
{
    /// <summary>
    /// The path of the hit object associated with the drawable.
    /// </summary>
    SliderPath? HitObjectPath { get; }
}
