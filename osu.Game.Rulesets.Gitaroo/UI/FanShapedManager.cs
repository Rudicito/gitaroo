using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Gitaroo.MathUtils;
using osu.Game.Rulesets.Gitaroo.Objects.Drawables;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.UI;

public partial class FanShapedManager : Container, IRequireHighFrequencyMousePosition
{
    [Resolved]
    private GitarooPlayfield playfield { get; set; }

    private DrawableTraceLine? currentTraceLine => playfield.CurrentDrawableTraceLine;

    private Vector2? mousePosition;

    private FanShaped fanShaped = null!;

    public FanShaped FanShaped
    {
        get => fanShaped;
        set => Child = fanShaped = value;
    }

    /// <summary>
    /// Is the FanShaped tracking a TraceLine?
    /// </summary>
    public bool Tracking { get; set; }

    private float direction;

    /// <summary>
    /// The current direction of the FanShaped, use for all the logics.
    /// </summary>
    public float Direction
    {
        get => direction;
        set
        {
            direction = value;
            FanShaped.Rotation = Tracking ? AngleTarget!.Value : value;
        }
    }

    private float angleArea = 70;
    private float halfAngleArea = 35;

    /// <summary>
    /// The angle portion considered correct.
    /// Exceeding it means missing the TraceLine.
    /// </summary>
    public float AngleArea
    {
        get => angleArea;
        set
        {
            angleArea = value;
            FanShaped.AngleArea = angleArea;
            halfAngleArea = value / 2;
        }
    }

    /// <summary>
    /// The angle the user must target to be allowed to play the HitObjects
    /// </summary>
    public float? AngleTarget;

    public FanShapedManager()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        FanShaped = new FanShaped();
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        mousePosition = e.MousePosition;
        return base.OnMouseMove(e);
    }

    protected override bool OnHover(HoverEvent e)
    {
        FanShaped.FadeIn();
        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        FanShaped.FadeOut();
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
        float start = Direction - halfAngleArea;
        float end = Direction + halfAngleArea;

        return AngleUtils.IsAngleBetween(angle, start, end);
    }

    public float? DeltaAngle;

    protected override void Update()
    {
        base.Update();

        if (mousePosition != null)
            Direction = AngleUtils.GetDegreesFromPosition(AnchorPosition, mousePosition.Value);

        if (currentTraceLine?.Direction != null) AngleTarget = currentTraceLine.Direction.Value;
        else AngleTarget = null;

        if (AngleTarget != null)
        {
            Tracking = CheckRotation(AngleTarget.Value);
            DeltaAngle = AngleUtils.GetAngleCloseness(Direction, AngleTarget.Value, halfAngleArea);
        }
        else
        {
            Tracking = false;
            DeltaAngle = null;
        }

        // Logger.Log($"DeltaAngle : {DeltaAngle}\n Tracking : {Tracking}");
    }
}
