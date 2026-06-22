using osu.Game.Rulesets.Gitaroo.Mods;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Gitaroo.Scoring;

public class GitarooScoreMultiplierCalculator : ScoreMultiplierCalculator
{
    public GitarooScoreMultiplierCalculator(ScoreMultiplierContext context)
        : base(context)
    {
        #region Automation

        Single<GitarooModAutopilot>(hasMultiplier: 0.1);

        #endregion
    }
}
