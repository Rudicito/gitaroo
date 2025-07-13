// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Gitaroo.Objects.Drawables;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Gitaroo.UI;

/// <remarks>
/// Lot of thing taken from osu!mania playfield
/// </remarks>
[Cached]
public partial class GitarooPlayfield : Playfield
{
    protected override GameplayCursorContainer CreateCursor() => new();

    private readonly FanShaped fanShaped;
    private readonly CenterCircle centerCircle;
    private readonly OrderedHitPolicy hitPolicy;

    public GitarooPlayfield()
    {
        fanShaped = new FanShaped();
        centerCircle = new CenterCircle();
        hitPolicy = new OrderedHitPolicy(HitObjectContainer);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRangeInternal(new Drawable[]
        {
            fanShaped,
            centerCircle,
            HitObjectContainer
        });

        RegisterPool<Note, DrawableNote>(10, 50);
        RegisterPool<HoldNote, DrawableHoldNote>(10, 50);
        RegisterPool<LineTrace, DrawableLineTrace>(2, 5);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        NewResult += OnNewResult;
    }

    protected override void Dispose(bool isDisposing)
    {
        // must happen before children are disposed in base call to prevent illegal accesses to the hit explosion pool.
        NewResult -= OnNewResult;

        base.Dispose(isDisposing);
    }

    protected override void OnNewDrawableHitObject(DrawableHitObject drawableHitObject)
    {
        base.OnNewDrawableHitObject(drawableHitObject);

        if (drawableHitObject is DrawableLineTraceHitObject dho)
        {
            dho.GetLineTrace = GetCurrentDrawableLineTrace;
            dho.CheckHittable = hitPolicy.IsHittable;
        }
    }

    internal void OnNewResult(DrawableHitObject judgedObject, JudgementResult result)
    {
        if (result.IsHit)
            hitPolicy.HandleHit(judgedObject);
    }

    public DrawableLineTrace? GetCurrentDrawableLineTrace(double time)
    {
        return HitObjectContainer.AliveObjects.OfType<DrawableLineTrace>().FirstOrDefault(x => x.IsActiveAtTime(time));
    }
}
