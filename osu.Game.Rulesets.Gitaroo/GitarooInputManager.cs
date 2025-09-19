// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.ComponentModel;
using System.Linq;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Game.Input.Bindings;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Gitaroo;

public partial class GitarooInputManager : RulesetInputManager<GitarooAction>
{
    public JoyAxis? JoyX => ((GitarooKeyBindingContainer)KeyBindingContainer).JoyX;
    public JoyAxis? JoyY => ((GitarooKeyBindingContainer)KeyBindingContainer).JoyY;

    public GitarooInputManager(RulesetInfo ruleset)
        : base(ruleset, 0, SimultaneousBindingMode.Unique)
    {
    }

    protected override KeyBindingContainer<GitarooAction> CreateKeyBindingContainer(RulesetInfo ruleset, int variant, SimultaneousBindingMode unique)
        => new GitarooKeyBindingContainer(ruleset, variant, unique);

    private partial class GitarooKeyBindingContainer : RulesetKeyBindingContainer
    {
        public JoyAxis? JoyX { get; private set; }
        public JoyAxis? JoyY { get; private set; }

        public GitarooKeyBindingContainer(RulesetInfo ruleset, int variant, SimultaneousBindingMode unique)
            : base(ruleset, variant, unique)
        {
        }

        protected override void ReloadMappings(IQueryable<RealmKeyBinding> realmKeyBindings)
        {
            base.ReloadMappings(realmKeyBindings);

            var listJoyUp = KeyBindings.Where(static b => b.GetAction<GitarooAction>() == GitarooAction.JoystickDown).ToList();
            var joyUp = listJoyUp[0].KeyCombination.Keys[0];

            var listJoyRight = KeyBindings.Where(static b => b.GetAction<GitarooAction>() == GitarooAction.JoystickRight).ToList();
            var joyRight = listJoyRight[0].KeyCombination.Keys[0];

            JoyX = getAxes(joyRight);
            JoyY = getAxes(joyUp);
        }

        private JoyAxis? getAxes(InputKey inputKey)
        {
            const int first_negative_joy_axis = (int)InputKey.FirstJoystickAxisNegativeButton;
            const int last_negative_joy_axis = first_negative_joy_axis + 63;
            const int first_positive_joy_axis = (int)InputKey.FirstJoystickAxisPositiveButton;
            const int last_positive_joy_axis = first_positive_joy_axis + 63;

            bool isNegative;

            int intInputKey = (int)inputKey;

            if (intInputKey >= first_negative_joy_axis && intInputKey <= last_negative_joy_axis) isNegative = true;
            else if (intInputKey >= first_positive_joy_axis && intInputKey <= last_positive_joy_axis) isNegative = false;
            else // This is not a JoystickAxis
            {
                return null;
            }

            // Convert from enum InputKey to enum JoystickAxisSource
            var joystickAxisSource = isNegative ? (JoystickAxisSource)(intInputKey - first_negative_joy_axis) : (JoystickAxisSource)(intInputKey - first_positive_joy_axis);

            return new JoyAxis
            {
                Source = joystickAxisSource,
                IsNegative = isNegative,
            };
        }
    }
}

public struct JoyAxis
{
    public JoystickAxisSource Source { get; init; }
    public bool IsNegative { get; init; }
}

public enum GitarooAction
{
    [Description("Left button")]
    LeftButton,

    [Description("Right button")]
    RightButton,

    [Description("Joystick Right")]
    JoystickRight,

    [Description("Joystick Down")]
    JoystickDown,
}
