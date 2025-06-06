﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Gitaroo.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.UI.Scrolling;

namespace osu.Game.Rulesets.Gitaroo.UI;

[Cached]
public partial class GitarooPlayfield : ScrollingPlayfield
{
    protected override GameplayCursorContainer CreateCursor() => new();

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRangeInternal(new Drawable[]
        {
            HitObjectContainer,
            new CenterCircle.CenterCircleContainer()
        });

        RegisterPool<Note, DrawableNote>(10, 50);
        RegisterPool<HoldNote, DrawableHoldNote>(10, 50);
        // RegisterPool<HeadNote, DrawableHoldNoteHead>(10, 50);
        // RegisterPool<TailNote, DrawableHoldNoteTail>(10, 50);
        // RegisterPool<HoldNoteBody, DrawableHoldNoteBody>(10, 50);
    }
}
