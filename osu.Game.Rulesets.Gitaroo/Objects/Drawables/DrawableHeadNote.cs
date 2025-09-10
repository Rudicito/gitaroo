using osu.Framework.Graphics;

namespace osu.Game.Rulesets.Gitaroo.Objects.Drawables;

public partial class DrawableHeadNote : DrawableNote
{
    public DrawableHeadNote()
        : this(null)
    {
    }

    public DrawableHeadNote(Note? hitObject)
        : base(hitObject)
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.TopLeft;
    }
}
