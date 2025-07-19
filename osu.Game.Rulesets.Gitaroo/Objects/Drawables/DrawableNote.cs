namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

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
