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
using static Quintessential.Serialization.PuzzleModel;

class patch_PuzzleEditorScreen {

	private Scrollbar scrollbar; // initializer is not merged

	[MonoModIgnore]
	private readonly PuzzleEditorScreenType type;
	[MonoModIgnore]
	private Maybe<Puzzle> selectedPuzzle;

	[MonoModIgnore]
    private static extern void RenderPermissionToggle(bool isAvailable, Puzzle puzzle, Vector2 pos, string name, PuzzlePermissions permissionFlag, bool isEnabled);

	[MonoModIgnore]
	[PatchPuzzleEditorScreen]
	public extern void RenderFrame(float param);

	[MonoModReplace]
	private void RenderEditor(bool isPersonal, Vector2 pos, Bounds2 bounds, Puzzle puzzle) {
		scrollbar ??= new();

		// reimplement this section
		Vector2 size = new(1516f, 922f);
		//Vector2 corner = (InputManager.screenSize / 2 - size / 2 + new Vector2(-2, -11)).Rounded();
		Bounds2 panelSize = Bounds2.WithSize(pos + new Vector2(0, 88 + 5), size + new Vector2(-152f + 78, -158f - 40 - 10));
		Bounds2 coverBounds = panelSize.Translated(new(80, 0));

		// add scrollbar/scroll region
		using (var _ = scrollbar.RenderScrollbar(panelSize.Min, panelSize.Size.CeilingToInt(), 0, -30)) {
			// clear scroll zone
			class_226.method_600(Color.Transparent);

			var nCorner = new Vector2(-12, scrollbar.scrollOffset - 95);

			//// CustomPermissions may have just not been set? TODO: find a better place for the "canonical" setter
			var conv = (patch_Puzzle)(object)puzzle;
			conv.CustomPermissions ??= new();

            VanillaEditorRender(isPersonal, nCorner, bounds, puzzle);

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
			if (UI.DrawCheckbox(rulesCorner + new Vector2(ruleSize.X * 0 + 5, ruleSize.Y * 1), "Enable Modded Content", conv.IsModdedPuzzle))
				conv.ConvertFormat(!conv.IsModdedPuzzle);
			if (conv.IsModdedPuzzle) {
				TextureRenderer.RenderText("If you uncheck this box, modded atoms and glyphs will not load properly after restarting", rulesCorner + new Vector2(5, ruleSize.Y * 1.5f), Assets.fonts.crimson_13, Color.Red, (TextAlignment)0, 1, 0.6f, float.MaxValue, float.MaxValue, 0, new Color(), null, int.MaxValue, false, true);
				//UI.DrawText("If you uncheck this box, modded atoms and glyphs will not load properly after restarting", rulesCorner + new Vector2(5, ruleSize.Y * 1.5f), UI.SubTitle, Color.Red, (TextAlignment)0);
			}
			// TODO: will probably move to a separate mod
			//UI.DrawCheckbox(rulesCorner + new Vector2(ruleSize.X * 1 + 5, ruleSize.Y * 1), "Allow Overlap", false);

			// modded categories, if enabled
			Vector2 cursor = rulesCorner + new Vector2(0, ruleSize.Y * 2.5f);
			if (conv.IsModdedPuzzle)
				foreach (var category in QApi.PuzzleOptions.GroupBy(k => k.SectionName)) {
					UIUtils.RenderScreenTitle(category.Key, cursor, 900, false, true);

					var idx = 0;
					foreach (var option in category) {
						// ReSharper disable once PossibleLossOfFraction
						Vector2 selectorPos = cursor + new Vector2(ruleSize.X * (idx % 4) + 5, ruleSize.Y * (idx / 4 + 1.5f));
						// TODO: other option types
						if (option.Type == PuzzleOptionType.Boolean) {
							bool enabled = conv.CustomPermissions.Contains(option.ID);
							if (UI.DrawCheckbox(selectorPos, option.Name, enabled)) {
								if (enabled)
									conv.CustomPermissions.Remove(option.ID);
								else
									conv.CustomPermissions.Add(option.ID);
								GameLogic.instance.workshopManager.RegenCustomVersion(puzzle);
							}
						} else if (option.Type == PuzzleOptionType.Atom) {
							var currentChoice = option.AtomIn(puzzle);
							if (DrawAtomSelector(selectorPos, option.Name, currentChoice ?? AtomTypes.repeat))
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

	private void VanillaEditorRender(bool isPersonal, Vector2 pos, Bounds2 bounds, Puzzle puzzle) {

        PuzzleEditorScreen.PuzzleCont puzzleCont = new PuzzleEditorScreen.PuzzleCont();
        puzzleCont.field_4622 = puzzle;
        UIUtils.RenderScreenTitle(Translations.Translate("Products"), pos + new Vector2(489f, 774f), 904, false, true);
        UIUtils.RenderScreenTitle(Translations.Translate("Reagents"), pos + new Vector2(489f, 522f), 904, false, true);
        UIUtils.RenderScreenTitle(Translations.Translate("Mechanisms and Glyphs"), pos + new Vector2(489f, 270f), 904, false, true);

        bool screenOpened = false;
        for (int i = 0; i < 2; i++) {
            PuzzleInputOutput[] array = ((i == 0) ? puzzleCont.field_4622.outputs : puzzleCont.field_4622.inputs);
            string text = ((i == 0) ? Translations.Translate("Create New Product").ToUpper() : Translations.Translate("Create New Reagent").ToUpper());
            for (int j = 0; j < 4; j++) {
                Bounds2 bounds2 = Bounds2.WithSize(pos + new Vector2(495f, 588f) + new Vector2((float)(j * 236), (float)((i == 0) ? (-28) : (-281))), new Vector2(226f, 201f));
                if (array.Length > j) {
                    TextureRenderer.Render(isPersonal ? Assets.textures.field_94.field_805 : Assets.textures.field_94.field_808, bounds2.Min);
                    bool flag = false;
                    if (isPersonal) {
                        Bounds2 bounds3 = Bounds2.WithSize(bounds2.Min + new Vector2(176f, 165f), Assets.textures.field_94.field_806.size.ToVector2());
                        bool flag2 = bounds3.Contains(InputManager.MousePos());
                        if (!flag2 && bounds2.Contains(InputManager.MousePos())) {
                            flag = true;
                            if (InputManager.IsClickPressed((MouseButtonType)1)) {
                                int I = i;
                                int J = j;
                                var moleculeEditorScreen = new MoleculeEditorScreen(array[J].molecule, I == 0, new Action<Molecule>(molecule => {
                                    (I == 0 ? puzzleCont.field_4622.outputs : puzzleCont.field_4622.inputs)[J].molecule = molecule;
                                    GameLogic.instance.workshopManager.RegenCustomVersion(puzzleCont.field_4622);
                                }));
                                ((patch_MoleculeEditorScreen)(object)moleculeEditorScreen).editing = (patch_Puzzle)(object)puzzle;
                                screenOpened = true;

                                GameLogic.instance.PushScreen(moleculeEditorScreen);
                                Assets.sounds.field_1821.method_28(1f);
                            }
                        }
                        TextureRenderer.Render(Assets.textures.field_94.field_806, bounds3.Min);
                        if (flag2) {
                            TextureRenderer.Render(Assets.textures.field_94.field_807, bounds3.Min);
                            if (InputManager.IsClickPressed((MouseButtonType)1)) {

                                int I = i;
                                int J = j;
                                GameLogic.instance.PushScreen(MessageBoxScreen.ConfirmBox(bounds, true, (i == 0) ? Translations.Translate("Do you really want to delete this product?") : Translations.Translate("Do you really want to delete this reagent?"), MaybeHelper.empty, (i == 0) ? Translations.Translate("Delete Product") : Translations.Translate("Delete Reagent"), Translations.Translate("Cancel"), () => {

                                    if (I == 0) {
                                        puzzleCont.field_4622.outputs = puzzleCont.field_4622.outputs.Take<PuzzleInputOutput>(J).Concat<PuzzleInputOutput>(puzzleCont.field_4622.outputs.Skip<PuzzleInputOutput>(J + 1)).ToArray<PuzzleInputOutput>();
                                    } else {
                                        puzzleCont.field_4622.inputs = puzzleCont.field_4622.inputs.Take<PuzzleInputOutput>(J).Concat<PuzzleInputOutput>(puzzleCont.field_4622.inputs.Skip<PuzzleInputOutput>(J + 1)).ToArray<PuzzleInputOutput>();
                                    }
                                    GameLogic.instance.workshopManager.RegenCustomVersion(puzzleCont.field_4622);
                                }, () => { }));
                                Assets.sounds.field_1821.method_28(1f);
                            }
                        }
                    }
                    Texture renderedTexture = Editor.RenderMoleculeForDisplay(array[j].molecule, i != 0, flag, new Vector2(156f, 146f), false, MaybeHelper.empty).GetTarget().renderedTexture;
                    Vector2 vector = (renderedTexture.size.ToVector2() / 2).Rounded();
                    Vector2 vector2 = bounds2.Center.Rounded() - vector + new Vector2(-8f, 0f);
                    TextureRenderer.Render(renderedTexture, vector2);


                    if (((patch_Puzzle)(object)puzzle).IsModdedPuzzle) {
                        Vector2 namePos = bounds2.BottomLeft + new Vector2(bounds2.Width / 2f - 7, -17);
                        var isElement = array[j].molecule.GetAtoms().Count == 1;
                        var fallbackPvw = "_(" + (isElement ? array[j].molecule.GetAtoms().Values.First().atomType.elementalName : "Unnamed") + ")_";
                        Bounds2 textArea = TextureRenderer.RenderText(array[j].molecule.displayName.GetOrDefault(Translations.Translate(fallbackPvw)), namePos, Assets.fonts.crimson_13, UI.TextColor, (TextAlignment)1, 1f, 0.6f, 236, 206, 0, new Color(), null, int.MaxValue, true, true);
                        if (textArea.Contains(Input.MousePos()) && Input.IsLeftClickPressed() && !screenOpened) {
                            screenOpened = true;
                            int J = j;
                            GameLogic.instance.PushScreen(MessageBoxScreenEx.textbox(bounds, Translations.Translate("Please enter a new name for this " + (i == 0 ? "product:" : "reagent:")), array[j].molecule.displayName.HasValue() ? array[j].molecule.displayName.GetValue() : (isElement ? array[j].molecule.GetAtoms().Values.First().atomType.elementalName : ""), Translations.Translate("Rename " + (i == 0 ? "Product" : "Reagent")), (string s) => {
                                array[J].molecule.displayName = Translations.Translate(s);
                                GameLogic.instance.workshopManager.RegenCustomVersion(puzzle);
                            }));
                            Assets.sounds.field_1821.method_28(1f);
                        }
                    }
                } else if (isPersonal) {
                    Vector2 vector3 = new Vector2(-2f, -3f);
                    TextureRenderer.Render(Assets.textures.field_94.field_802, bounds2.Min + vector3);
                    TextureRenderer.RenderText(text, bounds2.Center + new Vector2(-6f, 0f), Assets.fonts.crimson_13, class_181.field_1718, (TextAlignment)1, 1f, 0.6f, 120f, float.MaxValue, 0, default(Color), null, int.MaxValue, false, true);
                    if (bounds2.Contains(InputManager.MousePos())) {
                        TextureRenderer.Render(Assets.textures.field_94.field_803, bounds2.Min + vector3);
                        if (InputManager.IsClickPressed((MouseButtonType)1)) {

                            int I = i;
                            var moleculeEditorScreen =new MoleculeEditorScreen(new Molecule(), i == 0, new Action<Molecule>(molecule => {
                                if (I == 0) {
                                    puzzleCont.field_4622.outputs = puzzleCont.field_4622.outputs.Concat(new PuzzleInputOutput(molecule)).ToArray<PuzzleInputOutput>();
                                } else {
                                    puzzleCont.field_4622.inputs = puzzleCont.field_4622.inputs.Concat(new PuzzleInputOutput(molecule)).ToArray<PuzzleInputOutput>();
                                }
                                GameLogic.instance.workshopManager.RegenCustomVersion(puzzleCont.field_4622);
                            }));
                            ((patch_MoleculeEditorScreen)(object)moleculeEditorScreen).editing = (patch_Puzzle)(object)puzzle;

							GameLogic.instance.PushScreen(moleculeEditorScreen);
                            Assets.sounds.field_1821.method_28(1f);
                        }
                    }
                }
            }
        }
        bool flag3 = isPersonal && !PuzzleSelectScreen.HasDRM();
        Vector2 vector4 = pos + new Vector2(494f, 219f);
        RenderPermissionToggle(isPersonal, puzzleCont.field_4622, vector4 + GetPermissionTogglePos(0, 0), Translations.Translate("Glyph of Bonding###Shorter"), PuzzlePermissions.Bonder, false);
        RenderPermissionToggle(isPersonal, puzzleCont.field_4622, vector4 + GetPermissionTogglePos(1, 0), Translations.Translate("Glyph of Multi-bonding###Shorter"), PuzzlePermissions.SpeedBonder, false);
        RenderPermissionToggle(isPersonal, puzzleCont.field_4622, vector4 + GetPermissionTogglePos(2, 0), Translations.Translate("Glyph of Triplex-bonding###Shorter"), PuzzlePermissions.PrismaBonder, false);
        RenderPermissionToggle(isPersonal, puzzleCont.field_4622, vector4 + GetPermissionTogglePos(3, 0), Translations.Translate("Glyph of Unbonding###Shorter"), PuzzlePermissions.Unbonder, false);
        RenderPermissionToggle(isPersonal, puzzleCont.field_4622, vector4 + GetPermissionTogglePos(0, 1), Translations.Translate("Glyph of Calcification###Shorter"), PuzzlePermissions.Calcification, false);
        RenderPermissionToggle(isPersonal, puzzleCont.field_4622, vector4 + GetPermissionTogglePos(1, 1), Translations.Translate("Glyph of Duplication###Shorter"), PuzzlePermissions.Duplication, false);
        RenderPermissionToggle(isPersonal, puzzleCont.field_4622, vector4 + GetPermissionTogglePos(2, 1), PartTypes.berlosWheel.name, PuzzlePermissions.BaronWheel, false);
        RenderPermissionToggle(isPersonal, puzzleCont.field_4622, vector4 + GetPermissionTogglePos(3, 1), Translations.Translate("Glyph of Animismus###Shorter"), PuzzlePermissions.LifeAndDeath, false);
        RenderPermissionToggle(isPersonal, puzzleCont.field_4622, vector4 + GetPermissionTogglePos(0, 2), Translations.Translate("Glyph of Projection###Shorter"), PuzzlePermissions.Projection, false);
        RenderPermissionToggle(isPersonal, puzzleCont.field_4622, vector4 + GetPermissionTogglePos(1, 2), Translations.Translate("Glyph of Purification###Shorter"), PuzzlePermissions.Purification, false);
        RenderPermissionToggle(isPersonal, puzzleCont.field_4622, vector4 + GetPermissionTogglePos(2, 2), Translations.Translate("Glyph of Disposal###Shorter"), PuzzlePermissions.Disposal, false);
        RenderPermissionToggle(isPersonal, puzzleCont.field_4622, vector4 + GetPermissionTogglePos(3, 2), Translations.Translate("Glyphs of Quintessence"), PuzzlePermissions.Quintessence, false);
        RenderPermissionToggle(isPersonal, puzzleCont.field_4622, vector4 + GetPermissionTogglePos(0, 3), Translations.Translate("Glyph of Rejection###Shorter"), PuzzlePermissions.Rejection, flag3);
        RenderPermissionToggle(isPersonal, puzzleCont.field_4622, vector4 + GetPermissionTogglePos(1, 3), Translations.Translate("Glyph of Division###Shorter"), PuzzlePermissions.Division, flag3);
        RenderPermissionToggle(isPersonal, puzzleCont.field_4622, vector4 + GetPermissionTogglePos(2, 3), Translations.Translate("Glyph of Proliferation###Shorter"), PuzzlePermissions.Proliferation, flag3);
        RenderPermissionToggle(isPersonal, puzzleCont.field_4622, vector4 + GetPermissionTogglePos(3, 3), PartTypes.ravarisWheel.name, PuzzlePermissions.RavariWheel, flag3);
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