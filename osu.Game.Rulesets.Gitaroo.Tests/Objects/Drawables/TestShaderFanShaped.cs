// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Gitaroo.UI;
using osuTK;
using osuTK.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace osu.Game.Rulesets.Gitaroo.Tests.Objects.Drawables
{
    /// <remarks>
    /// Heavily based of TestSceneCircularProgress from osu!framework
    /// </remarks>
    public partial class TestShaderFanShaped : TestSceneOsuGitaroo
    {
        [Resolved]
        private IRenderer renderer { get; set; }

        private FanShapedCustom fanShapedSprite;

        // private int rotateMode;
        // private const double period = 4000;
        // private const double transition_period = 2000;

        private Texture gradientTextureHorizontal;
        private Texture gradientTextureVertical;
        private Texture gradientTextureBoth;

        [BackgroundDependencyLoader]
        private void load()
        {
            const int width = 128;

            var image = new Image<Rgba32>(width, 1);

            gradientTextureHorizontal = renderer.CreateTexture(width, 1, true);

            for (int i = 0; i < width; ++i)
            {
                float brightness = (float)i / (width - 1);
                image[i, 0] = new Rgba32((byte)(128 + (1 - brightness) * 127), (byte)(128 + brightness * 127), 128, 255);
            }

            gradientTextureHorizontal.SetData(new TextureUpload(image));

            image = new Image<Rgba32>(width, 1);

            gradientTextureVertical = renderer.CreateTexture(1, width, true);

            for (int i = 0; i < width; ++i)
            {
                float brightness = (float)i / (width - 1);
                image[i, 0] = new Rgba32((byte)(128 + (1 - brightness) * 127), (byte)(128 + brightness * 127), 128, 255);
            }

            gradientTextureVertical.SetData(new TextureUpload(image));

            image = new Image<Rgba32>(width, width);

            gradientTextureBoth = renderer.CreateTexture(width, width, true);

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    float brightness = (float)i / (width - 1);
                    float brightness2 = (float)j / (width - 1);
                    image[i, j] = new Rgba32(
                        (byte)(128 + (1 + brightness - brightness2) / 2 * 127),
                        (byte)(128 + (1 + brightness2 - brightness) / 2 * 127),
                        (byte)(128 + (brightness + brightness2) / 2 * 127),
                        255);
                }
            }

            gradientTextureBoth.SetData(new TextureUpload(image));

            // textureAtlasTexture = textures.Get("sample-texture");

            Box background;
            Container maskingContainer;

            Children = new Drawable[]
            {
                background = new Box
                {
                    Colour = FrameworkColour.GreenDark,
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0f,
                },
                maskingContainer = new Container
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = new Vector2(250),
                    CornerRadius = 20,
                    Child = fanShapedSprite = new FanShapedCustom
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(400)
                    }
                }
            };

            // AddStep("Forward", delegate { setRotationMode(1); });
            // AddStep("Backward", delegate { setRotationMode(2); });
            // AddStep("Transition Focus", delegate { setRotationMode(3); });
            // AddStep("Transition Focus 2", delegate { setRotationMode(4); });
            // AddStep("Forward/Backward", delegate { setRotationMode(0); });

            AddStep("Horizontal Gradient Texture", delegate { setTexture(1); });
            AddStep("Vertical Gradient Texture", delegate { setTexture(2); });
            AddStep("2D Gradient Texture", delegate { setTexture(3); });
            AddStep("Texture Atlas Texture", delegate { setTexture(4); });
            AddStep("White Texture", delegate { setTexture(0); });

            AddStep("Red Colour", delegate { setColour(1); });
            AddStep("Horzontal Gradient Colour", delegate { setColour(2); });
            AddStep("Vertical Gradient Colour", delegate { setColour(3); });
            AddStep("2D Gradient Colour", delegate { setColour(4); });
            AddStep("White Colour", delegate { setColour(0); });

            // AddStep("Forward Transform", delegate { transform(0); });
            // AddStep("Backward Transform", delegate { transform(1); });
            // AddStep("Fwd/Bwd Transform", delegate { transform(2); });
            // AddStep("Easing Transform", delegate { transform(3); });

            AddToggleStep("Toggle masking", m => maskingContainer.Masking = m);
            AddToggleStep("Toggle aspect ratio", r => fanShapedSprite.Size = r ? new Vector2(600, 400) : new Vector2(400));
            AddToggleStep("Toggle background", b => background.Alpha = b ? 1 : 0);

            AddSliderStep("Angle", 0, 180, fanShapedSprite.Angle, s => fanShapedSprite.Angle = s);
            AddSliderStep("Lines Width", 0, 0.1, fanShapedSprite.LinesWidth, s => fanShapedSprite.LinesWidth = s);
            AddSliderStep("Lines Alpha", 0f, 1f, 45f, s => fanShapedSprite.LinesAlpha = s);
            AddSliderStep("FanShaped Min Alpha", 0f, 1f, fanShapedSprite.FanShapedMinAlpha, s => fanShapedSprite.FanShapedMinAlpha = s);
            AddSliderStep("FanShaped Max Alpha", 0f, 1f, fanShapedSprite.FanShapedMaxAlpha, s => fanShapedSprite.FanShapedMaxAlpha = s);
            AddSliderStep("Scale", 0f, 2f, 1f, s => fanShapedSprite.Scale = new Vector2(s));
        }

        // protected override void Update()
        // {
        //     base.Update();
        //
        //     switch (rotateMode)
        //     {
        //         case 0:
        //             clock.Progress = Time.Current % (period * 2) / period - 1;
        //             break;
        //
        //         case 1:
        //             clock.Progress = Time.Current % period / period;
        //             break;
        //
        //         case 2:
        //             clock.Progress = Time.Current % period / period - 1;
        //             break;
        //
        //         case 3:
        //             clock.Progress = Time.Current % transition_period / transition_period / 5 - 0.1f;
        //             break;
        //
        //         case 4:
        //             clock.Progress = (Time.Current % transition_period / transition_period / 5 - 0.1f + 2) % 2 - 1;
        //             break;
        //     }
        // }

        // private void setRotationMode(int mode)
        // {
        //     fanShapedSprite.ClearTransforms();
        //     rotateMode = mode;
        // }

        private void setTexture(int textureMode)
        {
            switch (textureMode)
            {
                case 0:
                    fanShapedSprite.Texture = renderer.WhitePixel;
                    break;

                case 1:
                    fanShapedSprite.Texture = gradientTextureHorizontal;
                    break;

                case 2:
                    fanShapedSprite.Texture = gradientTextureVertical;
                    break;

                case 3:
                    fanShapedSprite.Texture = gradientTextureBoth;
                    break;

                // case 4:
                //     clock.Texture = textureAtlasTexture;
                //     break;
            }
        }

        private void setColour(int colourMode)
        {
            switch (colourMode)
            {
                case 0:
                    fanShapedSprite.Colour = new Color4(255, 255, 255, 255);
                    break;

                case 1:
                    fanShapedSprite.Colour = new Color4(255, 88, 88, 255);
                    break;

                case 2:
                    fanShapedSprite.Colour = new ColourInfo
                    {
                        TopLeft = new Color4(255, 128, 128, 255),
                        TopRight = new Color4(128, 255, 128, 255),
                        BottomLeft = new Color4(255, 128, 128, 255),
                        BottomRight = new Color4(128, 255, 128, 255),
                    };
                    break;

                case 3:
                    fanShapedSprite.Colour = new ColourInfo
                    {
                        TopLeft = new Color4(255, 128, 128, 255),
                        TopRight = new Color4(255, 128, 128, 255),
                        BottomLeft = new Color4(128, 255, 128, 255),
                        BottomRight = new Color4(128, 255, 128, 255),
                    };
                    break;

                case 4:
                    fanShapedSprite.Colour = new ColourInfo
                    {
                        TopLeft = new Color4(255, 128, 128, 255),
                        TopRight = new Color4(128, 255, 128, 255),
                        BottomLeft = new Color4(128, 128, 255, 255),
                        BottomRight = new Color4(255, 255, 255, 255),
                    };
                    break;
            }
        }

        // private void transform(int tf)
        // {
        //     setRotationMode(-1);
        //
        //     switch (tf)
        //     {
        //         case 0:
        //             clock.ProgressTo(0).Then().ProgressTo(1, 1000).Loop();
        //             break;
        //
        //         case 1:
        //             clock.ProgressTo(1).Then().ProgressTo(0, 1000).Loop();
        //             break;
        //
        //         case 2:
        //             clock.ProgressTo(0, 1000).Then().ProgressTo(1, 1000).Loop();
        //             break;
        //
        //         case 3:
        //             clock.ProgressTo(0).Then().ProgressTo(1, 1000, Easing.InOutQuart).Loop();
        //             break;
        //     }
        // }
    }

    internal partial class FanShapedCustom : FanShapedSprite
    {
        public new double LinesWidth
        {
            get => base.LinesWidth;
            set => base.LinesWidth = value;
        }

        public new float LinesAlpha
        {
            get => base.LinesAlpha;
            set => base.LinesAlpha = value;
        }

        public new float FanShapedMinAlpha
        {
            get => base.FanShapedMinAlpha;
            set => base.FanShapedMinAlpha = value;
        }

        public new float FanShapedMaxAlpha
        {
            get => base.FanShapedMaxAlpha;
            set => base.FanShapedMaxAlpha = value;
        }
    }
}
