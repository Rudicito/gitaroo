using System.Collections.Generic;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Judgements;

namespace osu.Game.Rulesets.Gitaroo.Judgements;

/// <remarks>
/// An exact copy of "osu.Game.Rulesets.Mania/Judgements/HoldNoteJudgementResult.cs"
/// </remarks>
public class HoldNoteJudgementResult : JudgementResult
{
    private Stack<(double time, bool holding, GitarooAction? action)> holdingState { get; } = new Stack<(double, bool, GitarooAction?)>();

    public HoldNoteJudgementResult(HoldNote hitObject, Judgement judgement)
        : base(hitObject, judgement)
    {
        holdingState.Push((double.NegativeInfinity, false, null));
    }

    private (double time, bool holding, GitarooAction? action) getLastReport(double currentTime)
    {
        while (holdingState.Peek().time > currentTime)
            holdingState.Pop();

        return holdingState.Peek();
    }

    public bool IsHolding(double currentTime) => getLastReport(currentTime).holding;
    public GitarooAction? MainKey(double currentTime) => getLastReport(currentTime).action;

    public bool DroppedHoldAfter(double time)
    {
        foreach (var state in holdingState)
        {
            if (state.time >= time && !state.holding)
                return true;
        }

        return false;
    }

    public void ReportHoldState(double currentTime, bool holding, GitarooAction? action = null)
    {
        var lastReport = getLastReport(currentTime);
        if (holding != lastReport.holding)
            holdingState.Push((currentTime, holding, action));
    }
}
