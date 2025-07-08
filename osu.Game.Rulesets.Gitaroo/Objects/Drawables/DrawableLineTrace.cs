namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public partial class DrawableLineTrace : DrawableGitarooHitObject<LineTrace>
{
    public DrawableLineTrace()
        : this(null)
    {
    }

    public DrawableLineTrace(LineTrace? hitObject)
        : base(hitObject)
    {
    }
}
