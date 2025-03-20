using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Gitaroo.UI.CenterCircle;

public partial class CenterCircle : Circle, IKeyBindingHandler<GitarooAction>
{
    private int downCount;

    private const double expand_duration = 400;

    private readonly ColourInfo notPressedColor = new ColourInfo
    {
        TopLeft = new Color4(183, 115, 229, byte.MaxValue),
        TopRight = new Color4(65, 97, 225, byte.MaxValue),
        BottomLeft = new Color4(10, 105, 246, byte.MaxValue),
        BottomRight = new Color4(75, 107, 250, byte.MaxValue),
        HasSingleColour = false
    };

    private readonly ColourInfo pressedColor = new ColourInfo
    {
        TopLeft = new Color4(182, 0, 228, byte.MaxValue),
        TopRight = new Color4(68, 0, 232, byte.MaxValue),
        BottomLeft = new Color4(7, 0, 246, byte.MaxValue),
        BottomRight = new Color4(74, 0, 248, byte.MaxValue),
        HasSingleColour = false
    };

    private readonly Color4 borderNotPressedColor = new Color4(34, 43, 117, byte.MaxValue);
    private readonly Color4 borderPressedColor = new Color4(52, 9, 121, byte.MaxValue);

    private const float not_pressed_scale = 1f;
    private const float pressed_scale = 1.15f;

    public CenterCircle()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Size = new Vector2(25);
        Colour = notPressedColor;
        Masking = true;
        BorderThickness = 2.625f;
        BorderColour = borderNotPressedColor;
    }

    internal void UpdateCircle(int downCount)
    {
        if (downCount > 0)
        {
            Colour = pressedColor;
            BorderColour = borderPressedColor;
            expand();
        }
        else
        {
            Colour = notPressedColor;
            BorderColour = borderNotPressedColor;
            contract();
        }
    }

    private void expand()
    {
        this.ScaleTo(not_pressed_scale)
            .ScaleTo(pressed_scale, expand_duration, Easing.OutElasticHalf);
    }

    private void contract()
    {
        this.ScaleTo(not_pressed_scale, expand_duration, Easing.OutQuad);
    }

    // Code below based of osu.Game.Rulesets.Osu/UI/Cursor/OsuCursorContainer.cs

    public bool OnPressed(KeyBindingPressEvent<GitarooAction> e)
    {
        switch (e.Action)
        {
            case GitarooAction.LeftButton:
            case GitarooAction.RightButton:
                downCount++;
                UpdateCircle(downCount);
                break;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<GitarooAction> e)
    {
        switch (e.Action)
        {
            case GitarooAction.LeftButton:
            case GitarooAction.RightButton:
                downCount = Math.Max(0, downCount - 1);

                if (downCount == 0)
                    UpdateCircle(downCount);
                break;
        }
    }
}
