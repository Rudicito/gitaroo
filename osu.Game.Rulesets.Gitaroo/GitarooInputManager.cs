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
    public (JoystickAxisSource joystickAxisSource, JoystickButton joystickButton, bool IsNegative)? JoyX => ((GitarooKeyBindingContainer)KeyBindingContainer).JoyX;
    public (JoystickAxisSource joystickAxisSource, JoystickButton joystickButton, bool IsNegative)? JoyY => ((GitarooKeyBindingContainer)KeyBindingContainer).JoyY;

    protected override KeyBindingContainer<GitarooAction> CreateKeyBindingContainer(RulesetInfo ruleset, int variant, SimultaneousBindingMode unique)
        => new GitarooKeyBindingContainer(ruleset, variant, unique);

    public GitarooInputManager(RulesetInfo ruleset)
        : base(ruleset, 0, SimultaneousBindingMode.Unique)
    {
    }

    private partial class GitarooKeyBindingContainer : RulesetKeyBindingContainer
    {
        public (JoystickAxisSource joystickAxisSource, JoystickButton joystickButton, bool IsNegative) JoyX { get; private set; }
        public (JoystickAxisSource joystickAxisSource, JoystickButton joystickButton, bool IsNegative) JoyY { get; private set; }

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

        private (JoystickAxisSource joystickAxisSource, JoystickButton joystickButton, bool IsNegative) getAxes(InputKey inputKey)
        {
            const int first_negative_joy_axis = 2049; // First negative Joystick axis in enum InputKey. +1024 for Positive axes
            const int first_positive_joy_axis = 3073; // First positive Joystick axis in enum InputKey
            const int input_key_to_joystick_button = -1025; // Add this to convert enum inputKey to enum joystickButton (only for joysticks axis)

            bool isNegative = false;

            int intInputKey = (int)inputKey;

            if (intInputKey is >= first_negative_joy_axis and < first_positive_joy_axis) isNegative = true;

            var joystickAxisSource = isNegative ? (JoystickAxisSource)(intInputKey - first_negative_joy_axis) : (JoystickAxisSource)(intInputKey - first_positive_joy_axis);

            // if isNegative
            // {
            //     joystickButtonNeg = (JoystickButton)(intInputKey + input_key_to_joystick_button);
            // }
            var joystickButton = (JoystickButton)(intInputKey + input_key_to_joystick_button);

            return (joystickAxisSource, joystickButton, isNegative);
        }
    }
}

public enum GitarooAction
{
    [Description("Left button")]
    LeftButton,

    [Description("Right button")]
    RightButton,

    // [Description("Joystick Up")]
    // JoystickUp,
    //
    // [Description("Joystick Left")]
    // JoystickLeft,

    [Description("Joystick Right")]
    JoystickRight,

    [Description("Joystick Down")]
    JoystickDown,
}
