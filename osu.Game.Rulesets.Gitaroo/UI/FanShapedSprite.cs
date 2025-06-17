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

        public override void ApplyState()
        {
            base.ApplyState();

            Angle = Math.Abs((float)Source.angle);

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

            private readonly UniformPadding8 Padding;
        }
    }
}


