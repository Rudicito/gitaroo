using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Gitaroo.Configuration;

namespace osu.Game.Rulesets.Gitaroo;

public partial class GitarooSettingsSubsection : RulesetSettingsSubsection
{
    private readonly Ruleset ruleset;

    private SettingsSlider<float> joystickDeadZone = null!;

    public GitarooSettingsSubsection(Ruleset ruleset)
        : base(ruleset)
    {
        this.ruleset = ruleset;
    }

    protected override LocalisableString Header => ruleset.Description;

    [BackgroundDependencyLoader]
    private void load()
    {
        var config = (GitarooRulesetConfigManager)Config;

        Children = new Drawable[]
        {
            joystickDeadZone = new SettingsSlider<float>
            {
                LabelText = "Joystick Deadzone",
                KeyboardStep = 0.01f,
                DisplayAsPercentage = true,
                Current = config.GetBindable<float>(GitarooRulesetSettings.DeadZoneJoystick),
            },
        };

        joystickDeadZone.SetNoticeText(
            "Make sure the other joystick deadzone setting in the Input category is set to a very low value to avoid constant snapping to up, right, left, or down, because of some limitation.",
            true);
    }
}
