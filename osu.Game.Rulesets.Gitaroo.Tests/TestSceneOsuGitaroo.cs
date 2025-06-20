using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Gitaroo.Tests;

public partial class TestSceneOsuGitaroo : OsuTestScene
{
    protected override Ruleset CreateRuleset() => new GitarooRuleset();
}
