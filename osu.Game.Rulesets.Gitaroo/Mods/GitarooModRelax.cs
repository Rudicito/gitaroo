using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Localisation;
using osu.Game.Input.Handlers;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Gitaroo.Objects.Drawables;
using osu.Game.Rulesets.Gitaroo.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Play;

namespace osu.Game.Rulesets.Gitaroo.Mods;

public class GitarooModRelax : ModRelax, IUpdatableByPlayfield, IApplicableToDrawableRuleset<GitarooHitObject>, IApplicableToPlayer, IHasNoTimedInputs
{
    public override LocalisableString Description => "You don't need to click, just move with the TraceLine.";

    public override Type[] IncompatibleMods
        => base.IncompatibleMods.Concat(new[] { typeof(GitarooModAutopilot) }).ToArray();

    private GitarooInputManager gitarooInputManager = null!;

    private ReplayInputHandler.ReplayState<GitarooAction> state = null!;
    private double lastStateChangeTime;

    private DrawableGitarooRuleset ruleset = null!;
    private PressHandler pressHandler = null!;

    private bool isDownState;
    private bool wasLeft;

    public void ApplyToDrawableRuleset(DrawableRuleset<GitarooHitObject> drawableRuleset)
    {
        ruleset = (DrawableGitarooRuleset)drawableRuleset;

        // grab the input manager for future use.
        gitarooInputManager = ruleset.KeyBindingInputManager;
    }

    public void ApplyToPlayer(Player player)
    {
        pressHandler = new PressHandler(this);
        gitarooInputManager.AllowGameplayInputs = false;
    }

    public void Update(Playfield playfield)
    {
        bool requiresHold = false;
        bool requiresHit = false;

        double time = playfield.Clock.CurrentTime;

        foreach (var h in playfield.HitObjectContainer.AliveObjects.OfType<DrawableTraceLineHitObject>())
        {
            // we are not yet close enough to the object.
            if (time < h.HitObject.StartTime)
                break;

            // already hit or beyond the hittable end time.
            if (h.IsHit || (h.HitObject is IHasDuration hasEnd && time > hasEnd.EndTime))
                continue;

            if (h.CheckFanShaped?.Invoke() == true)
            {
                switch (h)
                {
                    case DrawableNote note:
                        handleHit(note);
                        break;

                    case DrawableHoldNote holdNote:
                    {
                        if (!holdNote.Head.IsHit)
                            handleHit(holdNote);

                        requiresHold |= holdNote.IsActive;
                    }

                        break;
                }
            }
        }

        if (requiresHit)
        {
            changeState(false);
            changeState(true);
        }

        if (requiresHold)
            changeState(true);
        else if (isDownState && time - lastStateChangeTime > AutoGenerator.KEY_UP_DELAY)
            changeState(false);

        void changeState(bool down)
        {
            if (isDownState == down)
                return;

            isDownState = down;
            lastStateChangeTime = time;

            state = new ReplayInputHandler.ReplayState<GitarooAction>
            {
                PressedActions = new List<GitarooAction>()
            };

            if (down)
            {
                pressHandler.HandlePress(wasLeft);
                wasLeft = !wasLeft;
            }
            else
            {
                pressHandler.HandleRelease(wasLeft);
            }
        }

        void handleHit(DrawableTraceLineHitObject hitObject)
        {
            requiresHit |= hitObject.HitObject.HitWindows.CanBeHit(time - hitObject.HitObject.StartTime);
        }
    }

    private class PressHandler
    {
        private readonly GitarooModRelax mod;

        public PressHandler(GitarooModRelax mod)
        {
            this.mod = mod;
        }

        public void HandlePress(bool wasLeft)
        {
            mod.state.PressedActions.Add(wasLeft ? GitarooAction.LeftButton : GitarooAction.RightButton);
            mod.state.Apply(mod.gitarooInputManager.CurrentState, mod.gitarooInputManager);
        }

        public void HandleRelease(bool wasLeft)
        {
            mod.state.Apply(mod.gitarooInputManager.CurrentState, mod.gitarooInputManager);
        }
    }
}
