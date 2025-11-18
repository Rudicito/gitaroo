using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Gitaroo.Configuration;
using osu.Game.Rulesets.Gitaroo.Objects.Drawables;
using osu.Game.Rulesets.Gitaroo.Utils;
using osuTK;

namespace osu.Game.Rulesets.Gitaroo.UI;

public partial class FanShapedManager : Container
{
    /// <summary>
    /// Automatically move the FanShaped to the <see cref="AngleTarget"/>.
    /// </summary>
    public bool Auto { get; set; }

    [Resolved]
    private GitarooPlayfield playfield { get; set; } = null!;

    [Resolved(CanBeNull = true)]
    private GitarooRulesetConfigManager? rulesetConfig { get; set; }

    private DrawableTraceLine? currentTraceLine => playfield.CurrentDrawableTraceLine;

    private Vector2? mousePosition;

    /// <summary>
    /// The Vector2 of the joystick that playing.
    /// Can be <c>null</c> when the joystick is disabled/ignored.
    /// </summary>
    private Vector2? joystick;

    private FanShaped fanShaped = null!;

    private JoyAxis? joyX => inputManager.JoyX;
    private JoyAxis? joyY => inputManager.JoyY;

    private bool joystickPriority;

    private GitarooInputManager inputManager = null!;

    private IBindable<bool> joystickEnabled { get; set; } = null!;
    private IBindable<float> joystickDeadZone { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        joystickEnabled = rulesetConfig?.GetBindable<bool>(GitarooRulesetSettings.JoystickEnabled) ?? new Bindable<bool>(true);
        joystickDeadZone = rulesetConfig?.GetBindable<float>(GitarooRulesetSettings.JoystickDeadZone) ?? new Bindable<float>(0.6f);
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

    private float? direction;

    /// <summary>
    /// The current direction of the FanShaped in degrees.
    /// Used for all the tracking calculations.
    /// Can be <c>null</c> when there is no direction.
    /// </summary>
    public float? Direction
    {
        get => direction;
        set
        {
            if (value != null)
            {
                direction = value.Value;
                FanShaped.Rotation = value.Value;
                FanShaped.FadeIn();
            }

            else
            {
                direction = null;
                FanShaped.FadeOut();
            }

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
    /// Can be <c>null</c> when there is no TraceLine angle to target.
    /// </summary>
    public float? AngleTarget;

    public FanShapedManager()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        FanShaped = new FanShaped();
        FanShaped.FadeOut(true);
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        joystickPriority = false;
        mousePosition = e.MousePosition;
        return base.OnMouseMove(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        mousePosition = null;
        Direction = null;
    }

    protected override bool OnJoystickAxisMove(JoystickAxisMoveEvent e)
    {
        if (!joystickEnabled.Value)
            return base.OnJoystickAxisMove(e);

        if (joyX == null || joyY == null)
            return base.OnJoystickAxisMove(e);

        if (!(e.Axis.Source == joyX.Value.Source || e.Axis.Source == joyY.Value.Source))
            return base.OnJoystickAxisMove(e);

        joystick ??= Vector2.Zero;

        Vector2 currentJoystick = joystick.Value;

        if (e.Axis.Source == joyX.Value.Source)
        {
            currentJoystick.X = joyX.Value.IsNegative ? -e.Axis.Value : e.Axis.Value;
        }

        else if (e.Axis.Source == joyY.Value.Source)
        {
            currentJoystick.Y = joyY.Value.IsNegative ? -e.Axis.Value : e.Axis.Value;
        }

        joystick = currentJoystick;

        // Apply dead zone
        if (joystick.Value.Length < joystickDeadZone.Value || joystick == Vector2.Zero)
        {
            joystick = null;

            if (joystickPriority) Direction = null;
        }

        return base.OnJoystickAxisMove(e);
    }

    /// <summary>
    /// Checks whether the given angle (in degrees) is considered valid based on the <see cref="AngleArea"/>.
    /// </summary>
    /// <param name="angle">The angle to check, in degrees.</param>
    /// <returns>True if the angle is within the valid range; otherwise, false.</returns>
    public bool CheckRotation(float angle)
    {
        if (Direction == null) return false;

        float start = Direction.Value - halfAngleArea;
        float end = Direction.Value + halfAngleArea;

        return AngleUtils.IsAngleBetween(angle, start, end);
    }

    /// <summary>
    /// The normalized angular difference between the current direction and the target angle:
    /// Returns 0 when perfectly aligned, -1 when at maximum error to the left, and 1 when at maximum error to the right.
    /// </summary>
    public float? DeltaAngle;

    protected override void Update()
    {
        base.Update();

        AngleTarget = currentTraceLine?.Direction;

        UpdateInput();

        // Update the Tracking and DeltaAngle
        if (AngleTarget != null)
        {
            Tracking = CheckRotation(AngleTarget.Value);

            if (Direction == null)
                DeltaAngle = null;
            else
                DeltaAngle = AngleUtils.GetAngleCloseness(Direction.Value, AngleTarget.Value, halfAngleArea);
        }
        else
        {
            Tracking = false;
            DeltaAngle = null;
        }
    }

    protected void UpdateInput()
    {
        if (Auto)
        {
            Direction = AngleTarget;
            return;
        }

        if (joystickEnabled.Value)
        {
            if (joystick != null)
            {
                Direction = AngleUtils.GetDegreesFromPosition(Vector2.Zero, joystick.Value);
                joystickPriority = true;
            }
        }

        else
        {
            joystick = null;
            joystickPriority = false;
        }

        if (mousePosition != null && !joystickPriority)
            Direction = AngleUtils.GetDegreesFromPosition(AnchorPosition, mousePosition.Value, 180);
    }
}
