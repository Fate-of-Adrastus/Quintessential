// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable SuspiciousTypeConversion.Global

using MonoMod;
using Quintessential;
using Quintessential.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
internal class patch_WorkshopManager{
	public void method_2230(){
		((WorkshopManager)(object)this).ReloadWorkshopPuzzles();
		((WorkshopManager)(object)this).ReloadCustomPuzzles();
	}

	// make the Browse button a no-op rather than crashing
	public void ActivateWorkshopOverlay(){}

	// load YAML-based puzzles alongside binary ones
	private extern IEnumerable<Puzzle> orig_LoadPuzzlesOfFolder(string folder);
	private IEnumerable<Puzzle> LoadPuzzlesOfFolder(string folder){
        var orig = orig_LoadPuzzlesOfFolder(folder);

        string path = Path.Combine(class_269.field_2102, folder);
        foreach (var puzzleFilePath in Directory.EnumerateFiles(path, "*.puzzle.yaml")) {
            PuzzleModel model = YamlHelper.Deserializer.Deserialize<PuzzleModel>(File.ReadAllText(puzzleFilePath));
            Puzzle fromModel;
            try {
                fromModel = PuzzleModel.FromModel(model);
            } catch (Exception e) {
                Logger.Log($"Exception loading custom puzzle \"{model.ID}\":");
                Logger.Log(e);
                continue;
            }

            fromModel.uniqueCustomVersion = (uint)typeof(WorkshopManager).GetMethod("GetCustomVersion", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .Invoke(GameLogic.instance.workshopManager, new object[] { puzzleFilePath });
            // ReSharper disable once PossibleInvalidCastException
            ((patch_Puzzle)(object)fromModel).IsModdedPuzzle = true;
            orig.Concat(fromModel);
        }
        return orig;
	}

    // give YAML-based puzzles the right file location
    // used for both finding and saving, though saving in the correct format is handled in `Puzzle`
    private extern string orig_CustomPuzzlePath(Puzzle puzzle);
	[MonoModPublic]
	public string CustomPuzzlePath(Puzzle puzzle){
		return ((patch_Puzzle)(object)puzzle).IsModdedPuzzle
			? Path.Combine(class_269.field_2102, "custom", puzzle.puzzleId + ".puzzle.yaml")
			: orig_CustomPuzzlePath(puzzle);
	}
}