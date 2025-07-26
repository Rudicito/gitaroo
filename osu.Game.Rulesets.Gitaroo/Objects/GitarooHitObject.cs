// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Gitaroo.Judgements;
using osu.Game.Rulesets.Gitaroo.Scoring;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.Objects;

public class GitarooHitObject : HitObject
{
    /// <summary>
    /// The radius of hit objects (ie. the radius of a <see cref="Note"/>).
    /// </summary>
    public const float OBJECT_RADIUS = 10;

    /// <summary>
    /// The width and height any element participating in display of a Note (or similarly sized object) should be.
    /// </summary>
    public static readonly Vector2 OBJECT_DIMENSIONS = new Vector2(OBJECT_RADIUS * 2);

    public override required double StartTime { get; set; }

    public override Judgement CreateJudgement() => new GitarooJudgement();

    protected override HitWindows? CreateHitWindows() => new GitarooHitWindows();
}
