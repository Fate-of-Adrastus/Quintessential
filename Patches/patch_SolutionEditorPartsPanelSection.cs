using MonoMod;
using MonoMod.Utils;

using Quintessential;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
#pragma warning disable IDE1006 // Naming Styles

[MonoModPatch("SolutionEditorPartsPanel/ToolbarManager")]
class patch_SolutionEditorPartsPanelSection {

	// add our parts to the panel
	[MonoModIgnore]
	SolutionEditorPartsPanel partsPanel;
	
	public extern void orig_AddTypeToToolbar(List<PartTypeForToolbar> partToolbar, PartType partType);
	public void AddTypeToToolbar(List<PartTypeForToolbar> partToolbar, PartType partType) {
		// find the puzzle we're in
		DynamicData selfData = new(partsPanel);
		var sol = selfData.Get<SolutionEditorScreen>("solEditScr");
		Puzzle puzzle = sol.GetSolution().GetPuzzle();
		// check if we have the appropriate custom permissions
		var perms = (((patch_Puzzle)(object)puzzle).CustomPermissions ??= new());
		var checker = ((patch_PartType)(object)partType).CustomPermissionCheck;

		if(checker == null || checker(perms))
            orig_AddTypeToToolbar(partToolbar, partType);

		if(((patch_Puzzle)(object)puzzle).IsModdedPuzzle)
			foreach(var pair in QApi.PanelParts.Where(pair => partType.Equals(pair.Right)))
                AddTypeToToolbar(partToolbar, pair.Left);
	}
}