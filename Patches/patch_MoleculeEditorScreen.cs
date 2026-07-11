
#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
#pragma warning disable IDE1006


using Quintessential;
using MonoMod;
using System.Collections.Generic;

class patch_MoleculeEditorScreen
{

    internal patch_Puzzle editing;

    private static readonly Texture prevAtoms = AssetLoaderHelper.LoadTexture("Quintessential/editor_go_left");
    private static readonly Texture prevAtomsFaded = AssetLoaderHelper.LoadTexture("Quintessential/editor_go_left_faded");
    private static readonly Texture prevAtomsHover = AssetLoaderHelper.LoadTexture("Quintessential/editor_go_left_hover");
    private static readonly Texture nextAtoms = AssetLoaderHelper.LoadTexture("Quintessential/editor_go_right");
    private static readonly Texture nextAtomsFaded = AssetLoaderHelper.LoadTexture("Quintessential/editor_go_right_faded");
    private static readonly Texture nextAtomsHover = AssetLoaderHelper.LoadTexture("Quintessential/editor_go_right_hover");
    // for measuring stuff in debugging
    //private static readonly Texture dot = class_235.method_618(Color.Red);

    public static int currentPage = 0;


    private bool ShowExtraUI => editing is { IsModdedPuzzle: true } && Quintessential.QApi.ModAtomTypes.Count > 0;

    [PatchMoleculeEditorScreenAtomTray]
    public extern void orig_RenderFrame(float detalTime);
    public void RenderFrame(float detalTime)
    {
        orig_RenderFrame(detalTime);
        if (!ShowExtraUI)
        {
            currentPage = 0;
            return;
        }

        // This was being instantiated before all other mods could call LoadPuzzleContent, causing the list to be unpopulated.
        // This only occurred when Reductive Metallurgy Campaign is loaded for me, though I've had other mods on.
        // Thankfully it's only an integer division, but I should find a more sensible patch.
        int LastPage = (Quintessential.QApi.ModAtomTypes.Count + 14) / 15;

        Vector2 uiSize = new(1516f, 922f);
        Vector2 corner = (Input.ScreenSize() / 2 - uiSize / 2 + new Vector2(-2f, -11f)).Rounded();
        Vector2 lPos = corner + new Vector2(90f, 800f);
        Vector2 rPos = lPos;
        rPos.X += 350 - nextAtoms.size.X;
        bool inLeftBound = Bounds2.WithSize(lPos, prevAtoms.size.ToVector2()).Contains(Input.MousePos());
        bool inRightBound = Bounds2.WithSize(rPos, nextAtoms.size.ToVector2()).Contains(Input.MousePos());
        TextureRenderer.Render(currentPage > 0 ? inLeftBound ? prevAtomsHover : prevAtoms : prevAtomsFaded, lPos);
        TextureRenderer.Render(currentPage < LastPage ? inRightBound ? nextAtomsHover : nextAtoms : nextAtomsFaded, rPos);
        UI.DrawText($"{currentPage + 1}/{LastPage + 1}", corner + new Vector2(262f, 800f), UI.Text, UI.TextColor, (TextAlignment)0);
        //UI.DrawTexture(currentPage > 0 ? inLeftBound ? prevAtomsHover : prevAtoms : prevAtomsFaded, lPos);
        //UI.DrawTexture(currentPage < LastPage ? inRightBound ? nextAtomsHover : nextAtoms : nextAtomsFaded, rPos);
        //UI.DrawText($"{currentPage + 1}/{LastPage + 1}", corner + new Vector2(262f, 800f), UI.Text, UI.TextColor, TextAlignment.Centred);
        if (Input.IsLeftClickPressed() && (inLeftBound || inRightBound))
        {
            Assets.sounds.field_1821.method_28(1f);

            if (inLeftBound && currentPage > 0)
            {
                currentPage--;
            }

            if (inRightBound && currentPage < LastPage)
            {
                currentPage++;
            }
        }
    }

    [MonoMod.MonoModIgnore]
    [PatchMoleculeEditorScreenMoleculeError]
    public extern void MoleculeError(); 

    [MonoMod.MonoModIgnore]
    private extern void AtomTypeSelector(Vector2 pos, AtomType type, bool b);
    internal void DrawAtoms(Vector2 corner, Vector2 spacing)
    {
        bool useVanillaList = currentPage == 0;
        List<AtomType> atoms = useVanillaList ? new()
        {
            AtomTypes.salt,
            AtomTypes.air,
            AtomTypes.fire,
            AtomTypes.quicksilver,
            AtomTypes.water,
            AtomTypes.earth,
            AtomTypes.lead,
            AtomTypes.tin,
            AtomTypes.iron,
            AtomTypes.copper,
            AtomTypes.silver,
            AtomTypes.gold,
            AtomTypes.vitae,
            AtomTypes.mors,
            AtomTypes.quintessence
        } : QApi.ModAtomTypes;

        Vector2 pos = corner;
        bool showExtra = InputManager.IsModifierKeyHeld(0);
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                int index = (useVanillaList ? 0 : 15 * (currentPage - 1)) + 3 * y + x;
                if (index >= atoms.Count)
                {
                    goto outer;
                }
                this.AtomTypeSelector(pos, atoms[index], true);
                if (showExtra)
                {
                    bool hovering = Bounds2.WithSize(pos - new Vector2(30, 30), new Vector2(61, 61)).Contains(Input.MousePos());
                    if (hovering)
                    {
                        TextureRenderer.RenderText(atoms[index].defaultName, pos + new Vector2(0, -40), Assets.fonts.crimson_9_75, UI.TextColor, (TextAlignment)0, 1f, 0.6f, float.MaxValue, float.MaxValue, 0, new Color(), null, int.MaxValue, false, true);
                        //UI.DrawText(atoms[index].defaultName, pos + new Vector2(0, -40), Assets.fonts.crimson_9_75, UI.TextColor, (TextAlignment)0);
                    }
                }
                pos.X += spacing.X;
            }
            pos.X = corner.X;
            pos.Y += spacing.Y;
        }
    outer:
        // ...
        return;
    }
}