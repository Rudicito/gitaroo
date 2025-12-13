using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Screens.Play;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Gitaroo.UI;

public partial class FanShaped : Container
{
    [Resolved]
    private IGameplayClock? gameplayClock { get; set; }

    private readonly FanShapedSprite fanShapedSprite;
    private readonly Triangle leftArrow, rightArrow;

    private const float left_arrow_max_x = -86;
    private const float right_arrow_max_x = 86;
    private const float fan_shaped_max_y = 155;

    private static readonly Color4 tracked_colour = Color4.Orange;
    private static readonly Color4 not_tracked_colour = Color4.Cyan;

    private static readonly Color4 left_right_arrow_colour = Color4.Cyan;
    private static readonly Color4 middle_arrow_colour = Color4.White;

    private enum Fade
    {
        In,
        Out,
    }

    private Fade fade = Fade.In;

    private float angleArea = 70;

    public float AngleArea
    {
        get => angleArea;
        set
        {
            angleArea = value;
            fanShapedSprite.Angle = value;
        }
    }

    public FanShaped()
    {
        Size = new Vector2(fan_shaped_max_y * 2);
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        InternalChildren =
        [
            fanShapedSprite = new FanShapedSprite // Fan Shaped
            {
                Name = "Fan Shaped Sprite",
                Size = new Vector2(fan_shaped_max_y * 2),
                Angle = AngleArea,
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Colour = not_tracked_colour,
                Rotation = 0,
            },

            new Triangle // Middle arrow
            {
                Name = "Middle Arrow",
                Origin = Anchor.BottomCentre,
                Anchor = Anchor.Centre,
                Size = new Vector2(17, 50),
                Colour = middle_arrow_colour,
                Alpha = 0.5f,
                Rotation = 0,
            },

            leftArrow = new Triangle // Left Arrow
            {
                Name = "Left Arrow",
                Origin = Anchor.BottomLeft,
                Anchor = Anchor.Centre,
                Size = new Vector2(11, 13),
                Position = new Vector2(left_arrow_max_x, 5),
                Colour = left_right_arrow_colour,
                Alpha = 0.5f,
                Rotation = 0,
            },

            rightArrow = new Triangle // Right Arrow
            {
                Name = "Right Arrow",
                Origin = Anchor.BottomRight,
                Anchor = Anchor.Centre,
                Size = new Vector2(11, 13),
                Position = new Vector2(right_arrow_max_x, 5),
                Colour = left_right_arrow_colour,
                Alpha = 0.5f,
                Rotation = 0,
            }
        ];
    }

    public void FadeIn(bool now = false)
    {
        if (gameplayClock?.IsRewinding == true)
            now = true;

        if (now)
        {
            Alpha = 1;
            leftArrow.X = left_arrow_max_x;
            rightArrow.X = right_arrow_max_x;
            fade = Fade.In;
            return;
        }

        if (fade == Fade.In) return;

        fade = Fade.In;

        const float reset_value = 0.5f;
        const float down_time = 40;
        const float up_time = 750;

        this.Animate(t => t.FadeTo(Math.Min(t.Alpha - reset_value, 0), down_time, Easing.OutQuint)
        ).Then(t => t.FadeIn(up_time, Easing.OutQuint));

        leftArrow.Animate(t => t.MoveToX(Math.Max(t.X - reset_value * left_arrow_max_x, 0), down_time, Easing.OutQuint)
        ).Then(t => t.MoveToX(left_arrow_max_x, up_time, Easing.OutQuint));

        rightArrow.Animate(t => t.MoveToX(Math.Min(t.X - reset_value * right_arrow_max_x, 0), down_time, Easing.OutQuint)
        ).Then(t => t.MoveToX(right_arrow_max_x, up_time, Easing.OutQuint));
    }

    public void FadeOut(bool now = false)
    {
        if (gameplayClock?.IsRewinding == true)
            now = true;

        if (now)
        {
            Alpha = 0;
            leftArrow.X = 0;
            rightArrow.X = 0;
            fade = Fade.Out;
            return;
        }

        if (fade == Fade.Out) return;

        fade = Fade.Out;

        const float reset_value = 0.5f;

        const float down_time = 40;
        const float up_time = 750;

        this.Animate(t => t.FadeTo(Math.Min(t.Alpha + reset_value, 1), down_time, Easing.OutQuint)
        ).Then(t => t.FadeOut(up_time, Easing.OutQuint));

        leftArrow.Animate(t => t.MoveToX(Math.Max(t.X + reset_value * left_arrow_max_x, left_arrow_max_x), down_time, Easing.OutQuint)
        ).Then(t => t.MoveToX(0, up_time, Easing.OutQuint));

        rightArrow.Animate(t => t.MoveToX(Math.Min(t.X + reset_value * right_arrow_max_x, right_arrow_max_x), down_time, Easing.OutQuint)
        ).Then(t => t.MoveToX(0, up_time, Easing.OutQuint));
    }

    /// <summary>
    /// Update the delta variable of the <see cref="FanShapedSprite"/> (the main triangle)
    /// </summary>
    /// <param name="delta">The delta to send</param>
    public void UpdateDelta(float delta) => fanShapedSprite.Delta = delta;

    /// <summary>
    /// Update the IsActive variable of the <see cref="FanShapedSprite"/> (the main triangle).
    /// When <c>true</c>, shows the direction of the tracking.
    /// When <c>false</c>, just stay in normal color.
    /// </summary>
    /// <param name="value">The <c>bool</c> to send</param>
    public void UpdateActive(bool value) => fanShapedSprite.IsActive = value;
}
