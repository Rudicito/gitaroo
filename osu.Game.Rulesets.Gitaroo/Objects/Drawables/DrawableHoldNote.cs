namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public partial class DrawableHoldNote : DrawableGitarooHitObject<HoldNote>
{
    public DrawableHoldNote()
        : this(null)
    {
    }

    public DrawableHoldNote(HoldNote hitObject)
        : base(hitObject)
    {
        // AutoSizeAxes = Axes.Y;
    }
}
