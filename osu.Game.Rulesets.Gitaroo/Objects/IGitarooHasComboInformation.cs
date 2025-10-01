using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Rulesets.Gitaroo.Objects;

public interface IGitarooHasComboInformation : IHasComboInformation
{
    /// <summary>
    /// If the <see cref="GitarooHitObject"/> was an osu! spinner.
    /// </summary>
    bool WasSpinner { get; }
}
