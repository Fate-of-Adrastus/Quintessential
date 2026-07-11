#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE1006 // Naming Styles

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable ArrangeTypeModifiers

using MonoMod;
using Quintessential;
using Quintessential.Internal;
using System;
using System.Linq;

class patch_PuzzleEditorScreen{
		
	private Scrollbar scrollbar; // initializer is not merged

	[MonoModIgnore]
	private readonly PuzzleEditorScreenType type;
	[MonoModIgnore]
	private Maybe<Puzzle> selectedPuzzle;

	[MonoModIgnore]
	private extern void RenderPermissionToggle(bool isAvailable, Puzzle puzzle, Vector2 pos, string name, PuzzlePermissions permissionFlag, bool isEnabled);
    
    [MonoModIgnore]
	[PatchPuzzleEditorScreen]
	public extern void RenderFrame(float param);

	// name is used in MonoModRules
    private extern void orig_RenderEditor(bool isPersonal, Vector2 pos, Bounds2 bounds, Puzzle puzzle);

    private void RenderEditor(bool isPersonal, Vector2 pos, Bounds2 bounds, Puzzle puzzle) {
		scrollbar ??= new();
		
		// reimplement this section
		Vector2 size = new(1516f, 922f);
		//Vector2 corner = (InputManager.screenSize / 2 - size / 2 + new Vector2(-2, -11)).Rounded();
		Bounds2 panelSize = Bounds2.WithSize(pos + new Vector2(0, 88 + 5), size + new Vector2(-152f + 78, -158f - 40 - 10));
		Bounds2 coverBounds = panelSize.Translated(new(80, 0));

		// add scrollbar/scroll region
		using(var _ = scrollbar.RenderScrollbar(panelSize.Min, panelSize.Size.CeilingToInt(), 0, -30)){
			// clear scroll zone
			class_226.method_600(Color.Transparent);

			var nCorner = new Vector2(-12, scrollbar.scrollOffset - 95);

            //// CustomPermissions may have just not been set? TODO: find a better place for the "canonical" setter
            var conv = (patch_Puzzle)(object)puzzle;
            conv.CustomPermissions ??= new();

            orig_RenderEditor(isPersonal, nCorner, bounds, puzzle);

            Vector2 ruleSize = new(236, -37);
            Vector2 partsCorner = nCorner + new Vector2(494f, 219f);

            // instructions selection
            Vector2 instructionsCorner = new(nCorner.X + 489, partsCorner.Y + ruleSize.Y * 4);
            UIUtils.RenderScreenTitle(Translations.Translate("Instructions"), instructionsCorner, 900, false, true);

            InstructionType[] types = InstructionTypes.instructions;
            var i = 0;
            foreach (var type in types) {
                var basePos = instructionsCorner + new Vector2(50 + 60 * i, -60);
                var istructionPos = basePos;
                if (type.permissionCategory == PuzzlePermissions.None)
                    continue;
                bool enabled = puzzle.permissionFlags.HasFlag(type.permissionCategory);

                Texture @base;
                if (enabled)
                    @base = Assets.textures.field_99.field_706.field_716;
                else {
                    @base = Assets.textures.field_99.field_706.field_717;
                    istructionPos += new Vector2(3, -3);
                }

                bool hovered = Bounds2.WithSize(basePos, @base.size.ToVector2()).Contains(Input.MousePos());
                Texture highlight = Assets.textures.field_99.field_706.field_720;

                TextureRenderer.Render(@base, basePos);
                TextureRenderer.Render(type.enabledTexture, istructionPos + new Vector2(1, 2));
                if (hovered)
                    TextureRenderer.Render(highlight, istructionPos + new Vector2(2, 4));

                if (hovered && Input.IsLeftClickPressed()) {
                    puzzle.permissionFlags ^= type.permissionCategory;
                    GameLogic.instance.workshopManager.RegenCustomVersion(puzzle);
                }

                i++;
            }

            // quintessential rules
            var rulesCorner = instructionsCorner + new Vector2(0, ruleSize.Y * 3.5f);
			UIUtils.RenderScreenTitle(Translations.Translate("Quintessential Rules"), rulesCorner - new Vector2(0, ruleSize.Y * .5f), 900, false, true);
			if(UI.DrawCheckbox(rulesCorner + new Vector2(ruleSize.X * 0 + 5, ruleSize.Y * 1), "Enable Modded Content", conv.IsModdedPuzzle))
				conv.ConvertFormat(!conv.IsModdedPuzzle);
			if (conv.IsModdedPuzzle)
			{
				TextureRenderer.RenderText("If you uncheck this box, modded atoms and glyphs will not load properly after restarting", rulesCorner + new Vector2(5, ruleSize.Y * 1.5f), Assets.fonts.crimson_13, Color.Red, (TextAlignment)0, 1, 0.6f, float.MaxValue, float.MaxValue, 0, new Color(), null, int.MaxValue, false, true);
				//UI.DrawText("If you uncheck this box, modded atoms and glyphs will not load properly after restarting", rulesCorner + new Vector2(5, ruleSize.Y * 1.5f), UI.SubTitle, Color.Red, (TextAlignment)0);
			}
			// TODO: will probably move to a separate mod
			//UI.DrawCheckbox(rulesCorner + new Vector2(ruleSize.X * 1 + 5, ruleSize.Y * 1), "Allow Overlap", false);
			
			// modded categories, if enabled
			Vector2 cursor = rulesCorner + new Vector2(0, ruleSize.Y * 2.5f);
			if(conv.IsModdedPuzzle)
				foreach(var category in QApi.PuzzleOptions.GroupBy(k => k.SectionName)){
					UIUtils.RenderScreenTitle(category.Key, cursor, 900, false, true);

					var idx = 0;
					foreach(var option in category){
						// ReSharper disable once PossibleLossOfFraction
						Vector2 selectorPos = cursor + new Vector2(ruleSize.X * (idx % 4) + 5, ruleSize.Y * (idx / 4 + 1.5f));
						// TODO: other option types
						if(option.Type == PuzzleOptionType.Boolean){
							bool enabled = conv.CustomPermissions.Contains(option.ID);
							if(UI.DrawCheckbox(selectorPos, option.Name, enabled)){
								if(enabled)
									conv.CustomPermissions.Remove(option.ID);
								else
									conv.CustomPermissions.Add(option.ID);
								GameLogic.instance.workshopManager.RegenCustomVersion(puzzle);
							}
						}else if(option.Type == PuzzleOptionType.Atom){
							var currentChoice = option.AtomIn(puzzle);
							if(DrawAtomSelector(selectorPos, option.Name, currentChoice ?? AtomTypes.repeat))
								UI.OpenScreen(new AtomSelectScreen("Select: " + option.Name, type => {
									option.SetAtomIn(puzzle, type);
									GameLogic.instance.workshopManager.RegenCustomVersion(puzzle);
								}, currentChoice));
						}

						idx++;
					}

					var rows = (int)Math.Ceiling(idx / 4f);
					cursor += new Vector2(0, ruleSize.Y * (rows + 2));
				}

			// expand the scroll area to cover the entire displayed area
			// we're off by one row
			scrollbar.SetHeightAndClamp(nCorner.Y - cursor.Y + panelSize.Height - ruleSize.Y + 24);
		}
	}

    internal static Vector2 GetPermissionTogglePos(int x, int y) {
        return new Vector2((float)(236 * x), (float)(-37 * y));
    }

    // TODO: generalize?
    private static bool DrawAtomSelector(Vector2 pos, string label, AtomType atom){
        //Bounds2 labelBounds = UI.DrawText(label, pos + new Vector2(45f, 13f), UI.SubTitle, UI.TextColor, (TextAlignment)0);
        Bounds2 labelBounds = TextureRenderer.RenderText(label, pos + new Vector2(45f, 13f), Assets.fonts.crimson_13, UI.TextColor, (TextAlignment)0, 1, 0.6f,float.MaxValue, float.MaxValue,0,new Color(),null,int.MaxValue,true, true);
        Vector2 atomPos = pos + new Vector2(17, 16);
		const float scale = 0.7f;
		Editor.RenderAtom(atom, atomPos, scale, 1, 1, 1, -21, 0, null, null, false);

		if(Vector2.Distance(atomPos, Input.MousePos()) < (37 * scale) || labelBounds.Contains(Input.MousePos())){
			Vector2 outlinePos = (atomPos - Assets.textures.field_89.field_124.size.ToVector2() * scale / 2).Rounded();
			var tex = Assets.textures.field_89.field_124;
			TextureRenderer.Render(tex, Color.White, outlinePos, tex.size.ToVector2() * 0.7f);
			if(Input.IsLeftClickPressed()){
				Assets.sounds.field_1821.method_28(1);
				return true;
			}
		}
		return false;
	}

	private static bool DrawPuzzleButton(Puzzle p, Vector2 param_3552, int param_3025, bool param_3553, bool param_3554, bool param_4458, bool param_4459) {
		bool shift = Input.IsShiftHeld();
		string name = shift ? "ID: " + p.puzzleId.Replace("_", "\\_") : p.puzzleName;

		// draw the button
		ButtonDrawingLogic bdl = UIUtils.PuzzleButton(name, param_3552, param_3025, param_3553, param_3554);
        bool flag = bdl.RenderAndCheckIfPressed(param_4458, param_4459);
		if (shift)
		{
			// draw hash line
			TextureRenderer.RenderText("HASH: " + p.uniqueCustomVersion.ToString(), bdl.getBounds().Center + new Vector2(-15f, -24f), Assets.fonts.crimson_13, UI.TextColor, (TextAlignment)1, 1f, 0.6f, float.MaxValue, param_3025 - 75, 0, default, null, int.MaxValue, false, true);
		}
        return flag;
    }
}