// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Game.Rulesets.Gitaroo.UI;

namespace osu.Game.Rulesets.Gitaroo.Tests.Objects.Drawables
{
    public partial class TestShaderFanShaped : TestSceneOsuGame
    {
        public TestShaderFanShaped()
        {
            Add(new FanShapedSprite());
        }
    }
}
