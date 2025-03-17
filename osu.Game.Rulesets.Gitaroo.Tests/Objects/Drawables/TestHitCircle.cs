using osu.Game.Rulesets.Gitaroo.Objects.Drawables.Pieces;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Gitaroo.Tests.Objects.Drawables;

public partial class TestHitCircle : OsuTestScene
{
    public TestHitCircle()
    {
        Add(new HitCirclePiece());
    }
}
