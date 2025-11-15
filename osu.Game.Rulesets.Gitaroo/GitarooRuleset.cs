// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Configuration;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Gitaroo.Beatmaps;
using osu.Game.Rulesets.Gitaroo.Configuration;
using osu.Game.Rulesets.Gitaroo.Mods;
using osu.Game.Rulesets.Gitaroo.UI;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo;

public class GitarooRuleset : Ruleset
{
    public override string Description => "Gitaroo";

    public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod>? mods = null) => new DrawableGitarooRuleset(this, beatmap, mods);

    public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new GitarooBeatmapConverter(beatmap, this);

    public override IBeatmapProcessor CreateBeatmapProcessor(IBeatmap beatmap) => new BeatmapProcessor(beatmap);

    public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap) => new GitarooDifficultyCalculator(RulesetInfo, beatmap);

    public override IEnumerable<Mod> GetModsFor(ModType type)
    {
        switch (type)
        {
            case ModType.DifficultyReduction:
                return new Mod[]
                {
                    new GitarooModNoFail(),
                    new MultiMod(new GitarooModHalfTime(), new GitarooModDaycore())
                };

            case ModType.DifficultyIncrease:
                return new Mod[]
                {
                    new MultiMod(new GitarooModSuddenDeath(), new GitarooModPerfect()),
                    new MultiMod(new GitarooModDoubleTime(), new GitarooModNightcore()),
                    new ModAccuracyChallenge(),
                };

            // case ModType.Conversion:
            //     return new Mod[]
            //     {
            //     };

            case ModType.Automation:
                return new Mod[]
                {
                    new MultiMod(new GitarooModAutoplay(), new GitarooModCinema()),
                    new GitarooModRelax(),
                    new GitarooModAutopilot(),
                };

            case ModType.Fun:
                return new Mod[]
                {
                    new MultiMod(new ModWindUp(), new ModWindDown()),
                    new GitarooModMuted(),
                    new ModAdaptiveSpeed(),
                };

            default:
                return Array.Empty<Mod>();
        }
    }

    public override string ShortName => "gitaroo";

    // public override string PlayingVerb => base.PlayingVerb;

    public override IRulesetConfigManager CreateConfig(SettingsStore? settings) => new GitarooRulesetConfigManager(settings, RulesetInfo);

    public override RulesetSettingsSubsection CreateSettings() => new GitarooSettingsSubsection(this);

    public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => new[]
    {
        new KeyBinding(InputKey.Z, GitarooAction.LeftButton),
        new KeyBinding(InputKey.X, GitarooAction.RightButton),
        new KeyBinding(InputKey.Joystick2, GitarooAction.LeftButton),
        new KeyBinding(InputKey.Joystick3, GitarooAction.RightButton),
        new KeyBinding(InputKey.JoystickAxis2Positive, GitarooAction.JoystickDown),
        new KeyBinding(InputKey.JoystickAxis1Positive, GitarooAction.JoystickRight),
    };

    public override Drawable CreateIcon() => new GitarooIcon();

    protected override IEnumerable<HitResult> GetValidHitResults() => new[]
    {
        HitResult.Great,
        HitResult.Good,
        HitResult.Meh
    };

    // Leave this line intact. It will bake the correct version into the ruleset on each build/release.
    public override string RulesetAPIVersionSupported => CURRENT_RULESET_API_VERSION;
}

internal partial class GitarooIcon : CompositeDrawable
{
    internal GitarooIcon()
    {
        AutoSizeAxes = Axes.Both;

        InternalChildren =
        [
            new SpriteIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Icon = FontAwesome.Regular.Circle,
            },

            new SpriteIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Scale = new Vector2(0.525f),
                Icon = FontAwesome.Solid.Guitar,
            }
        ];
    }
}
