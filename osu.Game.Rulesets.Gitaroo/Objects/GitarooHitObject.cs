// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Game.Rulesets.Gitaroo.Judgements;
using osu.Game.Rulesets.Gitaroo.Scoring;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.Objects;

public class GitarooHitObject : HitObject, IGitarooHasComboInformation
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

    protected override HitWindows CreateHitWindows() => new GitarooHitWindows();

    public bool WasSpinner { get; set; }

    public virtual bool NewCombo { get; set; }

    public int ComboOffset { get; set; }

    private HitObjectProperty<int> indexInCurrentCombo;

    public Bindable<int> IndexInCurrentComboBindable => indexInCurrentCombo.Bindable;

    public int IndexInCurrentCombo
    {
        get => indexInCurrentCombo.Value;
        set => indexInCurrentCombo.Value = value;
    }

    private HitObjectProperty<int> comboIndex;

    public Bindable<int> ComboIndexBindable => comboIndex.Bindable;

    public int ComboIndex
    {
        get => comboIndex.Value;
        set => comboIndex.Value = value;
    }

    private HitObjectProperty<int> comboIndexWithOffsets;

    public Bindable<int> ComboIndexWithOffsetsBindable => comboIndexWithOffsets.Bindable;

    public int ComboIndexWithOffsets
    {
        get => comboIndexWithOffsets.Value;
        set => comboIndexWithOffsets.Value = value;
    }

    private HitObjectProperty<bool> lastInCombo;

    public Bindable<bool> LastInComboBindable => lastInCombo.Bindable;

    public virtual bool LastInCombo
    {
        get => lastInCombo.Value;
        set => lastInCombo.Value = value;
    }

    public void UpdateComboInformation(IHasComboInformation? lastObj)
    {
        // It's a copied implementation from OsuHitObject (and CatchHitObject), to ensure Gitaroo hit objects share the same combo behavior,
        // as the combo determines the object's color.

        // TraceLines are totally ignored, so we just copy the info of the lastObj to the TraceLine
        if (this is TraceLine)
        {
            ComboIndex = lastObj?.ComboIndex ?? 0;
            ComboIndexWithOffsets = lastObj?.ComboIndexWithOffsets ?? 0;
            // -1 because if TraceLine is the first object, the next not TraceLine will be then 0 (not 100% sure if it's right)
            IndexInCurrentCombo = (lastObj?.IndexInCurrentCombo) ?? -1;
            return;
        }

        IGitarooHasComboInformation? gitarooLastObj = lastObj as IGitarooHasComboInformation;

        int index = lastObj?.ComboIndex ?? 0;
        int indexWithOffsets = lastObj?.ComboIndexWithOffsets ?? 0;
        int inCurrentCombo = (lastObj?.IndexInCurrentCombo + 1) ?? 0;

        // osu! Spinner always start a new combo
        if (!WasSpinner && (NewCombo || gitarooLastObj == null || gitarooLastObj.WasSpinner))
        {
            inCurrentCombo = 0;
            index++;
            indexWithOffsets += ComboOffset + 1;

            if (lastObj != null)
                lastObj.LastInCombo = true;
        }

        ComboIndex = index;
        ComboIndexWithOffsets = indexWithOffsets;
        IndexInCurrentCombo = inCurrentCombo;
    }
}
