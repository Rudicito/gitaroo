﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Gitaroo.Judgements;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Gitaroo.Objects
{
    public class GitarooHitObject : HitObject
    {
        public override Judgement CreateJudgement() => new GitarooJudgement();
    }
}
