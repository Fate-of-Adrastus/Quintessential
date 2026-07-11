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
	private void DisplayEditorPanelScreen(){
		scrollbar ??= new();
		
		// reimplement this section
		Vector2 size = new(1516f, 922f);
		Vector2 corner = (InputManager.screenSize / 2 - size / 2 + new Vector2(-2, -11)).Rounded();
		Bounds2 panelSize = Bounds2.WithSize(corner + new Vector2(0, 88 + 5), size + new Vector2(-152f + 78, -158f - 40 - 10));
		Bounds2 coverBounds = panelSize.Translated(new(80, 0));

		// add scrollbar/scroll region
		using(var _ = scrollbar.RenderScrollbar(panelSize.Min, panelSize.Size.CeilingToInt(), 0, -30)){
			// clear scroll zone
			class_226.method_600(Color.Transparent);
			// draw headers
			var nCorner = new Vector2(-10, scrollbar.scrollOffset - 100);

			UIUtils.RenderScreenTitle(Translations.Translate("Products"), nCorner + new Vector2(489, 774), 900, false, true);
			UIUtils.RenderScreenTitle(Translations.Translate("Reagents"), nCorner + new Vector2(489, 506), 900, false, true);
			UIUtils.RenderScreenTitle(Translations.Translate("Mechanisms and Glyphs"), nCorner + new Vector2(489, 233), 900, false, true);

			Puzzle myPuzzle = selectedPuzzle.GetValue();
            // CustomPermissions may have just not been set? TODO: find a better place for the "canonical" setter
            var conv = (patch_Puzzle)(object)myPuzzle;
			conv.CustomPermissions ??= new();

			// draw inputs/outputs
			bool isCustomPuzzle = type == 0;
			bool screenOpened = false;
			for(var row = 0; row < 2; ++row){
				bool isProduct = row == 0;
				PuzzleInputOutput[] puzzleIOs = isProduct ? myPuzzle.outputs : myPuzzle.inputs;
				for(var column = 0; column < 4; ++column){
					Bounds2 puzzleIoBox = Bounds2.WithSize(nCorner + new Vector2(495f, 576f) + new Vector2(column * 235, isProduct ? -28f : -297f), new Vector2(226f, 226f));
					if(puzzleIOs.Length > column){
						TextureRenderer.Render(isCustomPuzzle ? Assets.textures.field_94.field_805 : Assets.textures.field_94.field_808, puzzleIoBox.Min);
						var isHover = false;
						var molecule = puzzleIOs[column].molecule;
						if(isCustomPuzzle){
							Bounds2 deleteBounds = Bounds2.WithSize(puzzleIoBox.Min + new Vector2(176f, 175f), Assets.textures.field_94.field_806.size.ToVector2());
							bool isDelete = deleteBounds.Contains(Input.MousePos());
							// open editor if clicked
							if(!isDelete && puzzleIoBox.Contains(Input.MousePos())){
								isHover = true;
								if(Input.IsLeftClickPressed()){
									int columnTemp = column; // otherwise it's modified after(?)
									var moleculeEditorScreen = new MoleculeEditorScreen(molecule, isProduct, value => {
										puzzleIOs[columnTemp].molecule = value;
										GameLogic.instance.workshopManager.RegenCustomVersion(myPuzzle);
									});
									((patch_MoleculeEditorScreen)(object)moleculeEditorScreen).editing = conv;
									screenOpened = true;
									GameLogic.instance.PushScreen(moleculeEditorScreen);
									Assets.sounds.field_1821.method_28(1f);
								}
							}

							TextureRenderer.Render(Assets.textures.field_94.field_806, deleteBounds.Min);
							// open "are you sure" menu if X is clicked
							if(isDelete){
								TextureRenderer.Render(Assets.textures.field_94.field_807, deleteBounds.Min);
								if(Input.IsLeftClickPressed()){
									int rowTemp = row;
									int columnTemp = column;
									GameLogic.instance.PushScreen(MessageBoxScreen.ConfirmBox(coverBounds, true, isProduct ? Translations.Translate("Do you really want to delete this product?") : Translations.Translate("Do you really want to delete this reagent?"), MaybeHelper.empty, isProduct ? Translations.Translate("Delete Product") : Translations.Translate("Delete Reagent"), Translations.Translate("Cancel"),
										() => {
											if(rowTemp == 0)
												myPuzzle.outputs = myPuzzle.outputs.Take(columnTemp).Concat(myPuzzle.outputs.Skip(columnTemp + 1)).ToArray();
											else
												myPuzzle.inputs = myPuzzle.inputs.Take(columnTemp).Concat(myPuzzle.inputs.Skip(columnTemp + 1)).ToArray();
											GameLogic.instance.workshopManager.RegenCustomVersion(myPuzzle);
										}, /* cancel is no-op */ () => { }));
									Assets.sounds.field_1821.method_28(1f);
								}
							}
						}

						Texture moleculeSprite = Editor.RenderMoleculeForDisplay(molecule, (uint)row > 0U, isHover, new Vector2(156f, 146f), false, MaybeHelper.empty).GetTarget().renderedTexture;
						Vector2 halfSize = (moleculeSprite.size.ToVector2() / 2).Rounded();
						var centre = puzzleIoBox.Center.Rounded() - halfSize;
						TextureRenderer.Render(moleculeSprite, centre + new Vector2(-8f, -10f));

						if(conv.IsModdedPuzzle){
							Vector2 namePos = puzzleIoBox.BottomLeft + new Vector2(puzzleIoBox.Width / 2f - 7, -17);
							var isElement = molecule.GetAtoms().Count == 1;
							var fallbackPvw = "_(" + (isElement ? molecule.GetAtoms().Values.First().atomType.elementalName : "Unnamed") + ")_";
							Bounds2 textArea = TextureRenderer.RenderText(molecule.displayName.GetOrDefault(Translations.Translate(fallbackPvw)), namePos, Assets.fonts.crimson_13, UI.TextColor, (TextAlignment)1, 1f, 0.6f, 236, 206, 0, new Color(), null, int.MaxValue, true, true);
                            //Bounds2 textArea = UI.DrawText(molecule.displayName.GetOrDefault(Translations.Translate(fallbackPvw)), namePos, Assets.fonts.crimson_13, UI.TextColor, (TextAlignment)1, maxWidth: 236, ellipsesCutoff: 206);
							if(textArea.Contains(Input.MousePos()) && Input.IsLeftClickPressed() && !screenOpened){
								screenOpened = true;
								GameLogic.instance.PushScreen(MessageBoxScreenEx.textbox(coverBounds, Translations.Translate("Please enter a new name for this " + (isProduct ? "product:" : "reagent:")),  molecule.displayName.HasValue() ? molecule.displayName.GetValue() : (isElement ? molecule.GetAtoms().Values.First().atomType.elementalName : ""), Translations.Translate("Rename " + (isProduct ? "Product" : "Reagent")), (string s) => {
									molecule.displayName = Translations.Translate(s);
                                    GameLogic.instance.workshopManager.RegenCustomVersion(myPuzzle);
                                }));
								Assets.sounds.field_1821.method_28(1f);
							}
						}
					}else if(isCustomPuzzle){
						Vector2 offset = new(-2f, -3f);
						TextureRenderer.Render(Assets.textures.field_94.field_802, puzzleIoBox.Min + offset);
						TextureRenderer.RenderText(isProduct ? Translations.Translate("Create New Product").ToUpper() : Translations.Translate("Create New Reagent").ToUpper(), puzzleIoBox.Center + new Vector2(-6f, -8f), Assets.fonts.crimson_13, class_181.field_1718, (TextAlignment)1, 1f, 0.6f, 120f, float.MaxValue, 0, new Color(), null, int.MaxValue, false, true);

						if(!puzzleIoBox.Contains(Input.MousePos())) continue;
						TextureRenderer.Render(Assets.textures.field_94.field_803, puzzleIoBox.Min + offset);

						if(!InputManager.IsClickPressed((MouseButtonType)1)) continue;
						int rowTemp = row; // otherwise it's modified after
						var moleculeEditorScreen = new MoleculeEditorScreen(new Molecule(), isProduct, value => {
							if(rowTemp == 0)
								myPuzzle.outputs = myPuzzle.outputs.Concat(new PuzzleInputOutput(value)).ToArray();
							else
								myPuzzle.inputs = myPuzzle.inputs.Concat(new PuzzleInputOutput(value)).ToArray();
							GameLogic.instance.workshopManager.RegenCustomVersion(myPuzzle);
						});
						((patch_MoleculeEditorScreen)(object)moleculeEditorScreen).editing = conv;
						GameLogic.instance.PushScreen(moleculeEditorScreen);
						Assets.sounds.field_1821.method_28(1f);
					}
				}
			}

			// draw vanilla rule checkboxes
            bool hasDRM = !PuzzleSelectScreen.HasDRM();
			Vector2 ruleSize = new(236, -37);
            Vector2 partsCorner = nCorner + new Vector2(494f, 219f);

            RenderPermissionToggle(true, myPuzzle, partsCorner + GetPermissionTogglePos(0, 0), Translations.Translate("Glyph of Bonding###Shorter"), PuzzlePermissions.Bonder, false);
            RenderPermissionToggle(true, myPuzzle, partsCorner + GetPermissionTogglePos(1, 0), Translations.Translate("Glyph of Multi-bonding###Shorter"), PuzzlePermissions.SpeedBonder, false);
            RenderPermissionToggle(true, myPuzzle, partsCorner + GetPermissionTogglePos(2, 0), Translations.Translate("Glyph of Triplex-bonding###Shorter"), PuzzlePermissions.PrismaBonder, false);
            RenderPermissionToggle(true, myPuzzle, partsCorner + GetPermissionTogglePos(3, 0), Translations.Translate("Glyph of Unbonding###Shorter"), PuzzlePermissions.Unbonder, false);
            RenderPermissionToggle(true, myPuzzle, partsCorner + GetPermissionTogglePos(0, 1), Translations.Translate("Glyph of Calcification###Shorter"), PuzzlePermissions.Calcification, false);
            RenderPermissionToggle(true, myPuzzle, partsCorner + GetPermissionTogglePos(1, 1), Translations.Translate("Glyph of Duplication###Shorter"), PuzzlePermissions.Duplication, false);
            RenderPermissionToggle(true, myPuzzle, partsCorner + GetPermissionTogglePos(2, 1), PartTypes.berlosWheel.name, PuzzlePermissions.BaronWheel, false);
            RenderPermissionToggle(true, myPuzzle, partsCorner + GetPermissionTogglePos(3, 1), Translations.Translate("Glyph of Animismus###Shorter"), PuzzlePermissions.LifeAndDeath, false);
            RenderPermissionToggle(true, myPuzzle, partsCorner + GetPermissionTogglePos(0, 2), Translations.Translate("Glyph of Projection###Shorter"), PuzzlePermissions.Projection, false);
            RenderPermissionToggle(true, myPuzzle, partsCorner + GetPermissionTogglePos(1, 2), Translations.Translate("Glyph of Purification###Shorter"), PuzzlePermissions.Purification, false);
            RenderPermissionToggle(true, myPuzzle, partsCorner + GetPermissionTogglePos(2, 2), Translations.Translate("Glyph of Disposal###Shorter"), PuzzlePermissions.Disposal, false);
            RenderPermissionToggle(true, myPuzzle, partsCorner + GetPermissionTogglePos(3, 2), Translations.Translate("Glyphs of Quintessence"), PuzzlePermissions.Quintessence, false);
            RenderPermissionToggle(true, myPuzzle, partsCorner + GetPermissionTogglePos(0, 3), Translations.Translate("Glyph of Rejection###Shorter"), PuzzlePermissions.Rejection, hasDRM);
            RenderPermissionToggle(true, myPuzzle, partsCorner + GetPermissionTogglePos(1, 3), Translations.Translate("Glyph of Division###Shorter"), PuzzlePermissions.Division, hasDRM);
            RenderPermissionToggle(true, myPuzzle, partsCorner + GetPermissionTogglePos(2, 3), Translations.Translate("Glyph of Proliferation###Shorter"), PuzzlePermissions.Proliferation, hasDRM);
            RenderPermissionToggle(true, myPuzzle, partsCorner + GetPermissionTogglePos(3, 3), PartTypes.ravarisWheel.name, PuzzlePermissions.RavariWheel, hasDRM);

            // instructions selection
			Vector2 instructionsCorner = new(nCorner.X + 489, partsCorner.Y + ruleSize.Y * 3);
            //UIUtils.RenderScreenTitle(Translations.Translate("Instructions"), instructionsCorner, 900, false, true);

            //InstructionType[] types = InstructionTypes.instructions;
            //var i = 0;
            //foreach(var type in types){
            //	var basePos = instructionsCorner + new Vector2(80 + 60 * i, -60);
            //	var pos = basePos;
            //	if(type.permissionCategory == PuzzlePermissions.None)
            //		continue;
            //	bool enabled = myPuzzle.permissionFlags.HasFlag(type.permissionCategory);

            //	Texture @base;
            //	if(enabled)
            //		@base = Assets.textures.field_99.field_706.field_716;
            //	else{
            //		@base = Assets.textures.field_99.field_706.field_717;
            //		pos += new Vector2(3, -3);
            //	}

            //	bool hovered = Bounds2.WithSize(basePos, @base.size.ToVector2()).Contains(Input.MousePos());
            //             Texture highlight = Assets.textures.field_99.field_706.field_720;

            //	TextureRenderer.Render(@base, basePos);
            //             TextureRenderer.Render(type.enabledTexture, pos + new Vector2(1, 2));
            //	if(hovered)
            //        TextureRenderer.Render(highlight, pos + new Vector2(2, 4));

            //	if(hovered && Input.IsLeftClickPressed()){
            //		myPuzzle.permissionFlags ^= type.permissionCategory;
            //		GameLogic.instance.workshopManager.RegenCustomVersion(myPuzzle);
            //	}

            //	i++;
            //}

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
						Vector2 pos = cursor + new Vector2(ruleSize.X * (idx % 4) + 5, ruleSize.Y * (idx / 4 + 1.5f));
						// TODO: other option types
						if(option.Type == PuzzleOptionType.Boolean){
							bool enabled = conv.CustomPermissions.Contains(option.ID);
							if(UI.DrawCheckbox(pos, option.Name, enabled)){
								if(enabled)
									conv.CustomPermissions.Remove(option.ID);
								else
									conv.CustomPermissions.Add(option.ID);
								GameLogic.instance.workshopManager.RegenCustomVersion(myPuzzle);
							}
						}else if(option.Type == PuzzleOptionType.Atom){
							var currentChoice = option.AtomIn(myPuzzle);
							if(DrawAtomSelector(pos, option.Name, currentChoice ?? AtomTypes.repeat))
								UI.OpenScreen(new AtomSelectScreen("Select: " + option.Name, type => {
									option.SetAtomIn(myPuzzle, type);
									GameLogic.instance.workshopManager.RegenCustomVersion(myPuzzle);
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