using System;
using System.Runtime.InteropServices;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Shaders.Types;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Gitaroo.UI;

public partial class FanShapedSprite : Sprite
{
    private double angle = 45;

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

    private Color4 trackedColour = Color4.Orange;

    public Color4 TrackedColour
    {
        get => trackedColour;
        set
        {
            if (trackedColour == value)
                return;

            trackedColour = value;

            if (IsLoaded)
                Invalidate(Invalidation.DrawNode);
        }
    }

    public bool Tracked { get; set; }

    private float delta;

    public float Delta
    {
        get => delta;
        set
        {
            if (delta == value)
                return;

            delta = value;

            if (IsLoaded)
                Invalidate(Invalidation.DrawNode);
        }
    }

    private double linesWidth = 0.005;

    protected double LinesWidth
    {
        get => linesWidth;
        set
        {
            if (!double.IsFinite(value))
                throw new ArgumentException($"{nameof(LinesWidth)} must be finite, but is {value}.");

            if (linesWidth == value)
                return;

            linesWidth = value;

            if (IsLoaded)
                Invalidate(Invalidation.DrawNode);
        }
    }

    private float linesAlpha = 1;

    protected float LinesAlpha
    {
        get => linesAlpha;
        set => alphaSetter(ref linesAlpha, value);
    }

    private float fanShapedMinAlpha = 0.1f;

    protected float FanShapedMinAlpha
    {
        get => fanShapedMinAlpha;
        set => alphaSetter(ref fanShapedMinAlpha, value);
    }

    private float fanShapedMaxAlpha = 1;

    protected float FanShapedMaxAlpha
    {
        get => fanShapedMaxAlpha;
        set => alphaSetter(ref fanShapedMaxAlpha, value);
    }

    private void alphaSetter(ref float field, float newValue)
    {
        newValue = float.Clamp(newValue, 0, 1);

        if (field == newValue)
            return;

        field = newValue;

        if (IsLoaded)
            Invalidate(Invalidation.DrawNode);
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
        protected float TrackedColourR { get; private set; }
        protected float TrackedColourG { get; private set; }
        protected float TrackedColourB { get; private set; }
        protected bool Tracked { get; private set; }
        protected float Delta { get; private set; }

        public override void ApplyState()
        {
            base.ApplyState();

            Angle = Math.Abs((float)Source.angle);
            LinesWidth = Math.Abs((float)Source.linesWidth);
            LinesAlpha = Math.Abs(Source.linesAlpha);
            FanShapedMinAlpha = Math.Abs(Source.fanShapedMinAlpha);
            FanShapedMaxAlpha = Math.Abs(Source.fanShapedMaxAlpha);
            TrackedColourR = Source.TrackedColour.R;
            TrackedColourG = Source.TrackedColour.G;
            TrackedColourB = Source.TrackedColour.B;
            Tracked = Source.Tracked;
            Delta = Source.Delta;

            // smoothstep looks too sharp with 1px, let's give it a bit more
            TexelSize = 1.5f / ScreenSpaceDrawQuad.Size.X;
        }

        private IUniformBuffer<FanShapedParameters>? parametersBuffer;

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
                TrackedColourR = TrackedColourR,
                TrackedColourG = TrackedColourG,
                TrackedColourB = TrackedColourB,
                Tracked = Tracked,
                Delta = Delta,
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
            public required UniformFloat Angle;
            public required UniformFloat TexelSize;
            public required UniformFloat LinesWidth;
            public required UniformFloat LinesAlpha;
            public required UniformFloat FanShapedMinAlpha;
            public required UniformFloat FanShapedMaxAlpha;
            public required UniformFloat TrackedColourR;
            public required UniformFloat TrackedColourG;
            public required UniformFloat TrackedColourB;
            public required UniformBool Tracked;
            public required UniformFloat Delta;

            private readonly UniformPadding4 Padding;
        }
    }
}


