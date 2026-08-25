using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Cil;
using MonoMod.InlineRT;
using Quintessential;
using System;
using System.Diagnostics;
using System.Linq;

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

	public void ContentInit(){
        orig_ContentInit();
		QuintessentialLoader.LoadPuzzleContent();
	}

    [MonoModILInject("ContentInit")]
    static void ContentInitBondTypeInit(MethodDefinition method, CustomAttribute attrib) {
        MonoModRule.Modder.Log("Patching bond type init");

        if (!method.HasBody) {
            throw new Exception("Unable to patch bond types init. (no body)");
        }

        ILCursor cursor = new(new ILContext(method));

        if (!cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchCall("BondTextures", "Init")
        )) {
            throw new Exception("Unable to patch bond types init. (no call)");
        }

        TypeDefinition holder = MonoModRule.Modder.FindType("Quintessential.BondAPI.BondTypes").Resolve();
        MethodDefinition call = holder.Methods.First((f) => f.Name == "InitBonds");

        cursor.Emit(OpCodes.Call, call);
    }
}