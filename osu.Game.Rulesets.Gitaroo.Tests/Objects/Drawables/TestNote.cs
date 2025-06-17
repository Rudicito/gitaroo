using osu.Game.Rulesets.Gitaroo.Objects.Drawables.Pieces;

namespace osu.Game.Rulesets.Gitaroo.Tests.Objects.Drawables;

public partial class TestNote : TestSceneOsuGitaroo
{
    public TestNote()
    {
        Add(new HitCirclePiece());
    }
}
