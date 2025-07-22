namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

/// <summary>
/// Visualises a <see cref="Note"/> hit object.
/// </summary>
public partial class DrawableNote : DrawableTraceLineHitObject<Note>
{
    public DrawableNote()
        : this(null)
    {
    }

    public DrawableNote(Note? hitObject)
        : base(hitObject)
    {
        // AutoSizeAxes = Axes.Y;
    }
}
