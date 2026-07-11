using MonoMod;
using Quintessential;

#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
#pragma warning disable IDE1006 // Naming Styles

class patch_GameLogic{

	// calls mod loading
	public extern void orig_GameInit();
	public void GameInit(){
		QuintessentialLoader.PreInit();
        orig_GameInit();
		QuintessentialLoader.PostLoad();
	}
    public extern void orig_GameUnload(int exitCode);
	public void GameUnload(int exitCode){
		QuintessentialLoader.Unload();
        orig_GameUnload(exitCode);
	}

	[PatchBondTypesInit]
	public extern void orig_ContentInit();
	public void ContentInit(){
        orig_ContentInit();
		QuintessentialLoader.LoadPuzzleContent();
	}
}