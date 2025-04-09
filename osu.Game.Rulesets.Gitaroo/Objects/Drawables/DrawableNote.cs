namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public partial class DrawableNote : DrawableGitarooHitObject<Note>
{
    public DrawableNote()
        : this(null)
    {
    }

    public DrawableNote(Note hitObject)
        : base(hitObject)
    {
        // AutoSizeAxes = Axes.Y;
    }
}
