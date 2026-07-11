using MonoMod;

#pragma warning disable IDE1006 // Naming Styles

[MonoModPatch("GlyphEffect")]
public class patch_GlyphEffect
{
    public Color Color;

    public patch_GlyphEffect WithColor(Color color)
    {
        Color = color;
        return this;
    }

    [PatchGlyphEffectConstructor]
    [MonoModConstructor]
    [MonoModIgnore]
    public extern void ctor(SolutionEditorBase solutionEditor, EffectTimescaleType timescaleType, Vector2 posOffset, Texture[] textures, float speed, Vector2 offset, float angleOffset, float delay = 0);
}