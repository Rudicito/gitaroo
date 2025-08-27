using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Gitaroo.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Gitaroo.Skinning.Default;

public partial class MainNotePiece : CompositeDrawable
{
    private static readonly Vector2 note_size = GitarooHitObject.OBJECT_DIMENSIONS;

    private readonly NotePiece notePiece;
    private readonly CircularContainer kiaiContainer;

    [Resolved]
    private DrawableHitObject drawableObject { get; set; } = null!;

    public MainNotePiece()
    {
        Size = note_size;

        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChildren = new Drawable[]
        {
            notePiece = new NotePiece
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = note_size
            },
            kiaiContainer = new CircularContainer
            {
                Masking = true,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = note_size,
                Child = new KiaiFlash
                {
                    RelativeSizeAxes = Axes.Both,
                }
            },
        };
    }

    private readonly IBindable<Color4> accentColourBindable = new Bindable<Color4>();

    [BackgroundDependencyLoader]
    private void load()
    {
        accentColourBindable.BindTo(drawableObject.AccentColour);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        accentColourBindable.BindValueChanged(colour =>
        {
            notePiece.AccentColor = colour.NewValue;
            kiaiContainer.Colour = colour.NewValue;
        }, true);
    }
}
