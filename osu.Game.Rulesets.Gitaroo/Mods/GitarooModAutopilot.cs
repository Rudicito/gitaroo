using System;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Gitaroo.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Gitaroo.Mods;

public class GitarooModAutopilot : Mod, IApplicableToDrawableRuleset<GitarooHitObject>
{
    public override string Name => "Autopilot";
    public override string Acronym => "AP";
    public override IconUsage? Icon => OsuIcon.ModAutopilot;
    public override ModType Type => ModType.Automation;
    public override LocalisableString Description => @"TraceLines automatically tracked - just follow the rhythm.";
    public override double ScoreMultiplier => 0.1;

    public override Type[] IncompatibleMods => new[]
    {
        typeof(ModRelax),
        typeof(ModAutoplay)
    };

    public void ApplyToDrawableRuleset(DrawableRuleset<GitarooHitObject> drawableRuleset)
    {
        var gitarooRuleset = (DrawableGitarooRuleset)drawableRuleset;

        gitarooRuleset.Playfield.FanShaped.Auto = true;
    }
}
