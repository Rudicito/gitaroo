using osu.Framework.Graphics;
using osu.Game.Rulesets.Judgements;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.UI;

public partial class DrawableGitarooJudgement : DrawableJudgement
{
    public DrawableGitarooJudgement()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        RelativeSizeAxes = Axes.Both;
        Size = Vector2.One;
    }
}
