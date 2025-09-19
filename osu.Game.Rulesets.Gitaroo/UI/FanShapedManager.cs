using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Gitaroo.Configuration;
using osu.Game.Rulesets.Gitaroo.MathUtils;
using osu.Game.Rulesets.Gitaroo.Objects.Drawables;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.UI;

public partial class FanShapedManager : Container
{
    [Resolved]
    private GitarooPlayfield playfield { get; set; } = null!;

    [Resolved(CanBeNull = true)]
    private GitarooRulesetConfigManager? rulesetConfig { get; set; }

    private DrawableTraceLine? currentTraceLine => playfield.CurrentDrawableTraceLine;

    private Vector2? mousePosition;

    private Vector2? joystick;

    private FanShaped fanShaped = null!;

    private JoyAxis joyX => inputManager.JoyX;
    private JoyAxis joyY => inputManager.JoyY;

    private bool joystickPriority;

    private GitarooInputManager inputManager = null!;

    private IBindable<float> deadZoneJoystick { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        deadZoneJoystick = rulesetConfig?.GetBindable<float>(GitarooRulesetSettings.DeadZoneJoystick) ?? new Bindable<float>(0.6f);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        inputManager = (GitarooInputManager)GetContainingInputManager()!;
    }

    public FanShaped FanShaped
    {
        get => fanShaped;
        set => Child = fanShaped = value;
    }

    /// <summary>
    /// Whether the FanShaped is currently tracking a TraceLine.
    /// </summary>
    public bool Tracking { get; set; }

    private float direction;

    /// <summary>
    /// The current direction of the FanShaped in degrees.
    /// Used for all the tracking calculations.
    /// </summary>
    public float Direction
    {
        get => direction;
        set
        {
            direction = value;
            FanShaped.Rotation = Tracking ? AngleTarget!.Value : value;

            FanShaped.SetColour(DeltaAngle);
        }
    }

    private float angleArea = 70;
    private float halfAngleArea = 35;

    /// <summary>
    /// The angle range (in degrees) in which tracking is considered valid.
    /// A larger value makes tracking more forgiving; a smaller value makes it stricter.
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
    /// The current TraceLine angle the user must target to track the TraceLine.
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
        joystickPriority = false;
        FanShaped.FadeIn();
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

    protected override bool OnJoystickAxisMove(JoystickAxisMoveEvent e)
    {
        if (!(e.Axis.Source == joyX.Source || e.Axis.Source == joyY.Source))
            return base.OnJoystickAxisMove(e);

        joystick ??= Vector2.Zero;

        Vector2 currentJoystick = joystick.Value;

        if (e.Axis.Source == joyX.Source)
        {
            currentJoystick.X = joyX.IsNegative ? -e.Axis.Value : e.Axis.Value;
        }

        else if (e.Axis.Source == joyY.Source)
        {
            currentJoystick.Y = joyY.IsNegative ? -e.Axis.Value : e.Axis.Value;
        }

        joystick = currentJoystick;

        // Apply dead zone
        if (joystick.Value.Length < deadZoneJoystick.Value || joystick == Vector2.Zero)
        {
            joystick = null;
            FanShaped.FadeOut();
        }

        else
        {
            FanShaped.FadeIn();
        }

        return base.OnJoystickAxisMove(e);
    }

    /// <summary>
    /// Checks whether the given angle (in degrees) is considered valid based on the <see cref="AngleArea"/>.
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public bool CheckRotation(float angle)
    {
        float start = Direction - halfAngleArea;
        float end = Direction + halfAngleArea;

        return AngleUtils.IsAngleBetween(angle, start, end);
    }

    /// <summary>
    /// Normalized closeness to the target angle:
    /// 1 = exact match, decreasing towards 0 as the difference approaches <see cref="halfAngleArea"/>.
    /// </summary>
    public float DeltaAngle;

    protected override void Update()
    {
        base.Update();

        if (joystick != null)
        {
            Direction = AngleUtils.GetDegreesFromPosition(Vector2.Zero, joystick.Value);
            joystickPriority = true;
        }

        if (mousePosition != null && !joystickPriority)
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
            DeltaAngle = 0;
        }
    }
}
