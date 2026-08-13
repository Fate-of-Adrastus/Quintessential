using MonoMod;
using Quintessential;
using static MonoMod.QuintessentialPatches;

#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
#pragma warning disable IDE1006 // Naming Styles

class patch_GameLogic{
	public extern void orig_GameInit();
    public extern void orig_GameUnload(int exitCode);
	public extern void orig_ContentInit();


	public void GameInit(){
		QuintessentialLoader.PreInit();
        orig_GameInit();
		QuintessentialLoader.PostInit();
	}

	public void GameUnload(int exitCode){
		QuintessentialLoader.Unload();
        orig_GameUnload(exitCode);
	}

	[MonoModILInject("QuintessentialPatches/" + nameof(PatchBondTypesInit))]
	public void ContentInit(){
        orig_ContentInit();
		QuintessentialLoader.LoadPuzzleContent();
	}
}