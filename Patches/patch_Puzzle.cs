using System.Collections.Generic;
using System.IO;
using MonoMod;
using Quintessential;
using Quintessential.Serialization;

class patch_Puzzle{
	
	// Custom puzzle data
	public HashSet<string> CustomPermissions = new();
	
	// Is modded content allowed in this puzzle?
	// Controls whether this is saved to/from a vanilla `.puzzle` file, or a Quintessential `.puzzle.yaml` file
	// Don't set this if you don't know what you're doing!
	public bool IsModdedPuzzle = false;

	public Maybe<PlacedConduit[]> EngineConduits = MaybeHelper.empty;
	public Maybe<Payloads> Payloads = MaybeHelper.empty;

	// Save using the right format, and set Steam user ID to 0
	[PatchPuzzleIdWrite]
	public extern void orig_SaveToFile(string path);

	// Save .puzzle or .puzzle.yaml
	public void SaveToFile(string path){
		if(IsModdedPuzzle)
			File.WriteAllText(path, YamlHelper.Serializer.Serialize(PuzzleModel.FromPuzzle((Puzzle)(object)this)));
		else
            orig_SaveToFile(path);
	}

	public static extern Puzzle orig_LoadFromFile(string path);
	public static Puzzle LoadFromFile(string path){
		if(Path.GetExtension(path) == ".yaml"){
			Puzzle p = PuzzleModel.FromModel(YamlHelper.Deserializer.Deserialize<PuzzleModel>(File.ReadAllText(path)));
			((patch_Puzzle)(object)p).IsModdedPuzzle = true;
			return p;
		}
		return orig_LoadFromFile(path);
	}

	public void ConvertFormat(bool modded){
		Puzzle self = (Puzzle)(object)this;
		WorkshopManager wm = GameLogic.instance.workshopManager;
		// delete
		File.Delete(((patch_WorkshopManager)(object)wm).CustomPuzzlePath(self));
		// update
		IsModdedPuzzle = modded;
		// save
		wm.RegenCustomVersion(self);
	}
}