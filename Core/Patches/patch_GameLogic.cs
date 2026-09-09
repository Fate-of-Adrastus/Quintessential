using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Cil;
using MonoMod.InlineRT;
using Quintessential;
using SDL2;
using System;
using System.Linq;

#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
#pragma warning disable IDE1006 // Naming Styles

class patch_GameLogic {
    [MonoModIgnore] public patch_Settings settingsData;
    public extern void orig_GameInit();
    public extern void orig_GameUnload(int exitCode);
	public extern void orig_ContentInit();


	public void GameInit(){
		QuintessentialLoader.PreInit();
        orig_GameInit();
		QuintessentialLoader.PostInit();
	}

	public void GameUnload(int exitCode) {
        Logger.Log("Starting mod unloading.");
        foreach (var mod in QuintessentialLoader.CodeMods)
            mod.Unload();

        Logger.Log("Finished unloading.");
        orig_GameUnload(exitCode);
	}

	public void ContentInit(){
        orig_ContentInit();
        QuintessentialLoader.ModContentInit();
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

    public void SetWindowOffsetsToDefaults(Window window) {
        SDL.SDL_SetWindowPosition(window.window, settingsData.windowXPos.Get(), settingsData.windowYPos.Get());
    }

    [MonoModILInject("GameInit")]
    public static void PatchWindowPositionSet(MethodDefinition method, CustomAttribute attribute) {

        MonoModRule.Modder.Log("Patching window position set");

        if (!method.HasBody) {
            throw new Exception("Unable to patch window position set. (no body)");
        }

        ILCursor cursor = new(new ILContext(method));

        if (!cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchCallvirt("GameLogic", "CreateWindow"),
            instr => instr.MatchStfld("GameLogic", "gameWindow")
        )) {
            throw new Exception("Unable to patch window position set. (no call)");
        }
        FieldReference window = (FieldReference)cursor.Prev.Operand;

        TypeDefinition holder = MonoModRule.Modder.FindType("GameLogic").Resolve();
        MethodDefinition call = holder.Methods.First((f) => f.Name == "SetWindowOffsetsToDefaults");

        cursor.EmitLdarg0();
        cursor.EmitLdarg0();
        cursor.EmitLdfld(window);
        cursor.EmitCallvirt(call);
    }
    [MonoModILInject("method_959")]
    public static void PatchWindowPositionSet2(MethodDefinition method, CustomAttribute attribute) {

        MonoModRule.Modder.Log("Patching window position set2");

        if (!method.HasBody) {
            throw new Exception("Unable to patch window position set2. (no body)");
        }
        TypeDefinition holder = MonoModRule.Modder.FindType("GameLogic").Resolve();
        MethodDefinition call = holder.Methods.First((f) => f.Name == "SetWindowOffsetsToDefaults");

        ILCursor cursor = new(new ILContext(method));

        if (!cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchCall("SDL2.SDL", "SDL_SetWindowPosition")
        )) {
            throw new Exception("Unable to patch window position set2. (no call)");
        }
        int last = cursor.Index;
        cursor.TryGotoPrev(MoveType.Before, instr => instr.MatchLdfld(out var _));
        cursor.RemoveRange(last - cursor.Index);
        cursor.EmitCallvirt(call);
        cursor.Index -= 2;
        cursor.EmitLdarg0();
    }

    [MonoModILInject("GameLoop")]
    public static void PatchWindowPositionSetOnExit(MethodDefinition method, CustomAttribute attribute) {

        MonoModRule.Modder.Log("Patching window position set");

        if (!method.HasBody) {
            throw new Exception("Unable to patch window position set. (no body)");
        }

        ILCursor cursor = new(new ILContext(method));

        if (!cursor.TryGotoNext(MoveType.Before,
            instr => instr.MatchLdarg0(),
            instr => instr.MatchLdfld("GameLogic","settingsData"),
            instr => instr.MatchCallvirt("Settings", "GetConfig"),
            instr => instr.MatchCallvirt("ConfigFile", "WriteFile")
        )) {
            throw new Exception("Unable to patch window position set. (no call)");
        }
        FieldReference settings = cursor.Next.Next.Operand as FieldReference;

        TypeDefinition holder = MonoModRule.Modder.FindType("Settings").Resolve();
        MethodDefinition call = holder.Methods.First((f) => f.Name == "SaveWindowPosToCurrent");

        TypeDefinition gameL = MonoModRule.Modder.FindType("GameLogic").Resolve();
        FieldDefinition window = gameL.Fields.First((f) => f.Name == "gameWindow");

        cursor.EmitLdarg0();
        cursor.EmitLdfld(settings);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(window);
        cursor.Emit(OpCodes.Callvirt, call);
    }
}