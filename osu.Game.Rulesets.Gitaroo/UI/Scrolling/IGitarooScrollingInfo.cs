using osu.Framework.Bindables;
using osu.Framework.Lists;
using osu.Game.Rulesets.Timing;
using osu.Game.Rulesets.UI.Scrolling;

namespace osu.Game.Rulesets.Gitaroo.UI.Scrolling;

public interface IGitarooScrollingInfo : IScrollingInfo
{
    IBindable<SortedList<MultiplierControlPoint>> ControlPoints { get; }
}
