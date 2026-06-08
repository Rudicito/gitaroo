namespace osu.Game.Rulesets.Gitaroo.Objects;

/// <summary>
/// A <see cref="GitarooHitObject"/> that is placed on a <see cref="TraceLine"/>.
/// </summary>
public class TraceLineHitObject : GitarooHitObject
{
    public TraceLine? TraceLine { get; set; }
}
