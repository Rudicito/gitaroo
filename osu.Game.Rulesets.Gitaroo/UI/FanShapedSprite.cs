using System;
using System.Runtime.InteropServices;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Shaders.Types;
using osu.Framework.Graphics.Sprites;

namespace osu.Game.Rulesets.Gitaroo.UI;

public partial class FanShapedSprite : Sprite
{
    private const double lines_width = 0.005;
    private const double lines_alpha = 1;
    private const double fan_shaped_min_alpha = 0.1;
    private const double fan_shaped_max_alpha = 1;

    private double angle;

    public double Angle
    {
        get => angle;
        set
        {
            if (!double.IsFinite(value))
                throw new ArgumentException($"{nameof(Angle)} must be finite, but is {value}.");

            if (angle == value)
                return;

            angle = value;

            if (IsLoaded)
                Invalidate(Invalidation.DrawNode);
        }
    }

    [BackgroundDependencyLoader]
    private void load(ShaderManager shaders, IRenderer renderer)
    {
        Texture ??= renderer.WhitePixel;
        TextureShader = shaders.Load(VertexShaderDescriptor.TEXTURE_2, "FanShaped");
    }

    protected override DrawNode CreateDrawNode() => new FanShapedDrawNode(this);

    protected class FanShapedDrawNode : SpriteDrawNode
    {
        public new FanShapedSprite Source => (FanShapedSprite)base.Source;

        public FanShapedDrawNode(FanShapedSprite source)
            : base(source)
        {
        }

        protected float Angle { get; private set; }
        protected float TexelSize { get; private set; }
        protected float LinesWidth { get; private set; }
        protected float LinesAlpha { get; private set; }
        protected float FanShapedMinAlpha { get; private set; }
        protected float FanShapedMaxAlpha { get; private set; }

        public override void ApplyState()
        {
            base.ApplyState();

            Angle = Math.Abs((float)Source.angle);
            LinesWidth = Math.Abs((float)lines_width);
            LinesAlpha = Math.Abs((float)lines_alpha);
            FanShapedMinAlpha = Math.Abs((float)fan_shaped_min_alpha);
            FanShapedMaxAlpha = Math.Abs((float)fan_shaped_max_alpha);

            // smoothstep looks too sharp with 1px, let's give it a bit more
            TexelSize = 1.5f / ScreenSpaceDrawQuad.Size.X;
        }

        private IUniformBuffer<FanShapedParameters> parametersBuffer;

        protected override void BindUniformResources(IShader shader, IRenderer renderer)
        {
            base.BindUniformResources(shader, renderer);

            parametersBuffer ??= renderer.CreateUniformBuffer<FanShapedParameters>();
            parametersBuffer.Data = new FanShapedParameters
            {
                Angle = Angle,
                TexelSize = TexelSize,
                LinesWidth = LinesWidth,
                LinesAlpha = LinesAlpha,
                FanShapedMinAlpha = FanShapedMinAlpha,
                FanShapedMaxAlpha = FanShapedMaxAlpha,
            };

            shader.BindUniformBlock("m_FanShapedParameters", parametersBuffer);
        }

        protected override bool CanDrawOpaqueInterior => false;

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            parametersBuffer?.Dispose();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct FanShapedParameters
        {
            public UniformFloat Angle;
            public UniformFloat TexelSize;
            public UniformFloat LinesWidth;
            public UniformFloat LinesAlpha;
            public UniformFloat FanShapedMinAlpha;
            public UniformFloat FanShapedMaxAlpha;

            private readonly UniformPadding8 Padding;
        }
    }
}


