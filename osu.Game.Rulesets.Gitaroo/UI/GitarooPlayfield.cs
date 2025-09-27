// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Gitaroo.Objects.Drawables;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.UI;

/// <remarks>
/// Lot of thing taken from osu!mania and osu! playfield
/// </remarks>
[Cached]
public partial class GitarooPlayfield : Playfield
{
    protected override GameplayCursorContainer CreateCursor() => new();

    private readonly FanShapedManager fanShaped;
    private readonly CenterCircle centerCircle;

    private readonly JudgementContainer<DrawableGitarooJudgement> judgementLayer;
    private readonly ProxyContainer traceLines;

    private readonly OrderedHitPolicy hitPolicy;

    private readonly JudgementPooler<DrawableGitarooJudgement> judgementPooler;

    public GitarooPlayfield()
    {
        InternalChildren = new Drawable[]
        {
            traceLines = new ProxyContainer { RelativeSizeAxes = Axes.Both },
            HitObjectContainer,
            fanShaped = new FanShapedManager { RelativeSizeAxes = Axes.Both },
            centerCircle = new CenterCircle(),
            judgementLayer = new JudgementContainer<DrawableGitarooJudgement>
            {
                RelativeSizeAxes = Axes.Both,
                Position = new Vector2(0, -150)
            },
        };

        hitPolicy = new OrderedHitPolicy(HitObjectContainer);

        AddInternal(judgementPooler = new JudgementPooler<DrawableGitarooJudgement>(new[]
        {
            HitResult.Great,
            HitResult.Good,
            HitResult.Meh,
            HitResult.Miss,
        }));
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RegisterPool<Note, DrawableNote>(10, 50);

        RegisterPool<HoldNote, DrawableHoldNote>(10, 50);
        RegisterPool<HeadNote, DrawableHeadNote>(10, 50);

        RegisterPool<TraceLine, DrawableTraceLine>(2, 5);
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

        if (drawableHitObject is DrawableTraceLineHitObject dho)
        {
            dho.GetTraceLine = GetTraceLine;
            dho.CheckHittable = IsHittable;
        }

        else if (drawableHitObject is DrawableTraceLine dtl)
        {
            dtl.SetCurrentTraceLine = SetCurrentDrawableTraceLine;
        }

        drawableHitObject.OnLoadComplete += onDrawableHitObjectLoaded;
    }

    private void onDrawableHitObjectLoaded(Drawable drawable)
    {
        switch (drawable)
        {
            case DrawableTraceLine:
                traceLines.Add(drawable.CreateProxy());
                break;
        }
    }

    /// <summary>
    /// Whether the <see cref="DrawableTraceLineHitObject"/> can be hit, considering the FanShaped tracking status and the HitPolicy
    /// </summary>
    /// <param name="hitObject">The <see cref="DrawableTraceLineHitObject"/> to check.</param>
    /// <param name="time">The time to check.</param>
    /// <returns></returns>
    public bool IsHittable(DrawableHitObject hitObject, double time) => fanShaped.Tracking && hitPolicy.IsHittable(hitObject, time);

    internal void OnNewResult(DrawableHitObject judgedObject, JudgementResult result)
    {
        if (!judgedObject.DisplayResult || !DisplayJudgements.Value)
            return;

        if (result.IsHit)
            hitPolicy.HandleHit(judgedObject);

        judgementLayer.Clear(false);
        judgementLayer.Add(judgementPooler.Get(result.Type, j => j.Apply(result, judgedObject))!);
    }

    public DrawableTraceLine? CurrentDrawableTraceLine;

    public DrawableTraceLine? GetTraceLine(double time)
    {
        return HitObjectContainer.AliveObjects.OfType<DrawableTraceLine>().FirstOrDefault(x => x.IsActiveAtTime(time));
    }

    public void SetCurrentDrawableTraceLine(DrawableTraceLine traceLine)
    {
        CurrentDrawableTraceLine = traceLine;
    }

    private partial class ProxyContainer : LifetimeManagementContainer
    {
        public void Add(Drawable proxy) => AddInternal(proxy);
    }
}
