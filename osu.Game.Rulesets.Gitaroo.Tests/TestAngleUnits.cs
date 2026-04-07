using NUnit.Framework;
using osu.Game.Rulesets.Gitaroo.Utils;

namespace osu.Game.Rulesets.Gitaroo.Tests;

public partial class TestAngleUnits : TestSceneOsuGitaroo
{
    [Test]
    public void TestIsAngleBetween()
    {
        Assert.That(AngleUtils.IsAngleBetween(50, 30, 60), Is.True);
        Assert.That(AngleUtils.IsAngleBetween(20, 30, 60), Is.False);

        Assert.That(AngleUtils.IsAngleBetween(-3, -10, 10), Is.True);
        Assert.That(AngleUtils.IsAngleBetween(3, -10, 10), Is.True);
        Assert.That(AngleUtils.IsAngleBetween(-40, -30, 10), Is.False);
        Assert.That(AngleUtils.IsAngleBetween(40, -30, 10), Is.False);

        Assert.That(AngleUtils.IsAngleBetween(10, 320, 20), Is.True);
        Assert.That(AngleUtils.IsAngleBetween(310, 320, 20), Is.False);
    }

    [Test]
    public void TestGetAngleCloseness()
    {
        Assert.That(AngleUtils.GetAngleCloseness(50, 50, 20), Is.EqualTo(0f));
        Assert.That(AngleUtils.GetAngleCloseness(110, 50, 20), Is.EqualTo(1f));
        Assert.That(AngleUtils.GetAngleCloseness(0, -10, 20), Is.EqualTo(0.5f));
        Assert.That(AngleUtils.GetAngleCloseness(350, -10, 20), Is.EqualTo(0f));
    }
}
