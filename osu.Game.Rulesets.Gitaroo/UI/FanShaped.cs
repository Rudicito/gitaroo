using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Gitaroo.MathUtils;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Gitaroo.UI;

public partial class FanShaped : Container
{
    public float FanShapedRotation
    {
        get => fanShapedRotation;
        private set
        {
            fanShapedRotation = value;
            rotatingContainer.Rotation = fanShapedRotation;
        }
    }

    private float fanShapedRotation;

    private readonly Triangle leftArrow, rightArrow;
    private readonly Container rotatingContainer;
    private const float left_arrow_max_x = -86;
    private const float right_arrow_max_x = 86;
    private const float fan_shaped_max_y = 155;
    public float FanShapedAngle = 70;
    private readonly float halfFanShapedAngle;

    public FanShaped()
    {
        halfFanShapedAngle = FanShapedAngle / 2;

        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        InternalChildren =
        [
            rotatingContainer = new Container
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.TopCentre,
                Alpha = 0f,
                Rotation = FanShapedRotation,
                Children = new Sprite[]
                {
                    new FanShapedSprite // Fan Shaped
                    {
                        Size = new Vector2(fan_shaped_max_y * 2),
                        Angle = FanShapedAngle,
                        Origin = Anchor.Centre,
                        Anchor = Anchor.TopCentre,
                        Colour = Color4.Cyan,
                        Rotation = 180,
                    },

                    new Triangle // Middle arrow
                    {
                        Origin = Anchor.BottomCentre,
                        Anchor = Anchor.TopCentre,
                        Size = new Vector2(17, 50),
                        Colour = Color4.White,
                        Alpha = 0.5f,
                        Rotation = 180,
                    },

                    leftArrow = new Triangle // Left Arrow
                    {
                        Origin = Anchor.BottomLeft,
                        Anchor = Anchor.TopCentre,
                        Size = new Vector2(11, 13),
                        Position = new Vector2(left_arrow_max_x, 5),
                        Colour = Color4.Cyan,
                        Alpha = 0.5f,
                        Rotation = 180,
                    },

                    rightArrow = new Triangle // Right Arrow
                    {
                        Origin = Anchor.BottomRight,
                        Anchor = Anchor.TopCentre,
                        Size = new Vector2(11, 13),
                        Position = new Vector2(right_arrow_max_x, 5),
                        Colour = Color4.Cyan,
                        Alpha = 0.5f,
                        Rotation = 180,
                    }
                }
            }
        ];
    }

    private void fanShapeFadeIn()
    {
        const float reset_value = 0.5f;
        const float down_time = 40;
        const float up_time = 750;

        rotatingContainer.Animate(t => t.FadeTo(Math.Min(t.Alpha - reset_value, 0), down_time, Easing.OutQuint)
        ).Then(t => t.FadeIn(up_time, Easing.OutQuint));

        leftArrow.Animate(t => t.MoveToX(Math.Max(t.X - reset_value * left_arrow_max_x, 0), down_time, Easing.OutQuint)
        ).Then(t => t.MoveToX(left_arrow_max_x, up_time, Easing.OutQuint));

        rightArrow.Animate(t => t.MoveToX(Math.Min(t.X - reset_value * right_arrow_max_x, 0), down_time, Easing.OutQuint)
        ).Then(t => t.MoveToX(right_arrow_max_x, up_time, Easing.OutQuint));
    }

    private void fanShapeFadeOut()
    {
        const float reset_value = 0.5f;

        const float down_time = 40;
        const float up_time = 750;

        rotatingContainer.Animate(t => t.FadeTo(Math.Min(t.Alpha + reset_value, 1), down_time, Easing.OutQuint)
        ).Then(t => t.FadeOut(up_time, Easing.OutQuint));

        leftArrow.Animate(t => t.MoveToX(Math.Max(t.X + reset_value * left_arrow_max_x, left_arrow_max_x), down_time, Easing.OutQuint)
        ).Then(t => t.MoveToX(0, up_time, Easing.OutQuint));

        rightArrow.Animate(t => t.MoveToX(Math.Min(t.X + reset_value * right_arrow_max_x, right_arrow_max_x), down_time, Easing.OutQuint)
        ).Then(t => t.MoveToX(0, up_time, Easing.OutQuint));
    }

    protected override bool OnHover(HoverEvent e)
    {
        fanShapeFadeIn();
        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        fanShapeFadeOut();
    }

    private float getDegreesFromPosition(Vector2 a, Vector2 b)
    {
        Vector2 direction = b - a;
        float rawAngle = MathHelper.RadiansToDegrees(MathF.Atan2(direction.Y, direction.X));

        return Angle.NormalizeAngle(rawAngle - 90);
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        FanShapedRotation = getDegreesFromPosition(AnchorPosition, e.MousePosition);
        return base.OnMouseMove(e);
    }

    // todo: implement joystick support
    //
    // protected override bool OnJoystickAxisMove(JoystickAxisMoveEvent e)
    // {
    //     return base.OnJoystickAxisMove(e);
    // }

    /// <summary>
    /// Check if the given angle is considered in the FanShaped area
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public bool CheckRotation(float angle)
    {
        float start = FanShapedRotation - halfFanShapedAngle;
        float end = FanShapedRotation + halfFanShapedAngle;

        return Angle.IsAngleBetween(angle, start, end);
    }
}
