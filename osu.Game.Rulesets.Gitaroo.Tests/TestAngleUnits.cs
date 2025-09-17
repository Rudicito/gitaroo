using NUnit.Framework;
using osu.Game.Rulesets.Gitaroo.MathUtils;

namespace osu.Game.Rulesets.Gitaroo.Tests;

public partial class TestAngleUnits : TestSceneOsuGitaroo
{
    [Test]
    public void TestIsAngleBetween()
    {
        Assert.True(AngleUtils.IsAngleBetween(50, 30, 60));
        Assert.False(AngleUtils.IsAngleBetween(20, 30, 60));

        Assert.True(AngleUtils.IsAngleBetween(-3, -10, 10));
        Assert.True(AngleUtils.IsAngleBetween(3, -10, 10));
        Assert.False(AngleUtils.IsAngleBetween(-40, -30, 10));
        Assert.False(AngleUtils.IsAngleBetween(40, -30, 10));

        Assert.True(AngleUtils.IsAngleBetween(10, 320, 20));
        Assert.False(AngleUtils.IsAngleBetween(310, 320, 20));
    }

    [Test]
    public void TestGetAngleCloseness()
    {
        Assert.AreEqual(1, AngleUtils.GetAngleCloseness(50, 50, 20));
        Assert.AreEqual(0, AngleUtils.GetAngleCloseness(110, 50, 20));
        Assert.AreEqual(0.5f, AngleUtils.GetAngleCloseness(0, -10, 20));
        Assert.AreEqual(1f, AngleUtils.GetAngleCloseness(350, -10, 20));
    }
}
