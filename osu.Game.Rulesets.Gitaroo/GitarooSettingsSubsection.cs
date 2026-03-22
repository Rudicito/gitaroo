using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Gitaroo.Configuration;

namespace osu.Game.Rulesets.Gitaroo;

public partial class GitarooSettingsSubsection : RulesetSettingsSubsection
{
    private readonly Ruleset ruleset;

    private readonly Bindable<SettingsNote.Data?> joystickNote = new Bindable<SettingsNote.Data?>(
        new SettingsNote.Data(
            "Make sure the other joystick deadzone setting in the Input category is set to a very low value to avoid constant snapping to up, right, left, or down, because of some limitation.",
            SettingsNote.Type.Warning
        )
    );

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
            new SettingsItemV2(new FormCheckBox
            {
                Caption = "Joystick Enabled",
                Current = config.GetBindable<bool>(GitarooRulesetSettings.JoystickEnabled)
            }),

            new SettingsItemV2(new FormSliderBar<float>
            {
                Caption = "Joystick Deadzone",
                KeyboardStep = 0.01f,
                DisplayAsPercentage = true,
                Current = config.GetBindable<float>(GitarooRulesetSettings.JoystickDeadZone)
            })
            {
                Note = { BindTarget = joystickNote }
            }
        };
    }
}
