using osu.Game.Configuration;
using osu.Game.Rulesets.Configuration;

namespace osu.Game.Rulesets.Gitaroo.Configuration;

public class GitarooRulesetConfigManager : RulesetConfigManager<GitarooRulesetSettings>
{
    public GitarooRulesetConfigManager(SettingsStore? settings, RulesetInfo ruleset, int? variant = null)
        : base(settings, ruleset, variant)
    {
    }

    protected override void InitialiseDefaults()
    {
        base.InitialiseDefaults();

        SetDefault(GitarooRulesetSettings.JoystickEnabled, true);
        SetDefault(GitarooRulesetSettings.JoystickDeadZone, 0.6f, 0f, 1f, 0.005f);
    }
}

public enum GitarooRulesetSettings
{
    JoystickEnabled,
    JoystickDeadZone
}
