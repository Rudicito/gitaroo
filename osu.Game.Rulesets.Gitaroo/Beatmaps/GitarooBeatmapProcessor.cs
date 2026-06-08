using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Gitaroo.Objects;

namespace osu.Game.Rulesets.Gitaroo.Beatmaps;

public class GitarooBeatmapProcessor : BeatmapProcessor
{
    public GitarooBeatmapProcessor(IBeatmap beatmap)
        : base(beatmap)
    {
    }

    public override void PreProcess()
    {
        IGitarooHasComboInformation? lastObj = null;

        // For sanity, ensures that both the first hitobject and the first hitobject after a spinner start a new combo.
        // This is normally enforced by the legacy decoder, but is not enforced by the editor.
        foreach (var obj in Beatmap.HitObjects.OfType<IGitarooHasComboInformation>())
        {
            if (!obj.WasSpinner && (lastObj == null || lastObj.WasSpinner))
                obj.NewCombo = true;
            lastObj = obj;
        }

        base.PreProcess();

        // Populate TraceLineHitObject
        var traceLines = Beatmap.HitObjects.OfType<TraceLine>().ToList();

        foreach (var hitObject in Beatmap.HitObjects.OfType<TraceLineHitObject>())
        {
            hitObject.TraceLine = traceLines.FirstOrDefault(t =>
                t.StartTime <= hitObject.StartTime && t.EndTime >= hitObject.StartTime);
        }
    }
}
