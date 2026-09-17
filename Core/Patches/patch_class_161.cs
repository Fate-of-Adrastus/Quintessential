using Mono.Cecil;
using MonoMod;
using MonoMod.Cil;

[MonoModPatch("class_161")]
internal class patch_class_161 {

    [MonoModILInject("method_402")]
    private static void PatchSetSaveFolder(MethodDefinition method, CustomAttribute attribute) {
        ILCursor cursor = new(new ILContext(method));

        cursor.GotoNext(MoveType.Before, instr => instr.MatchLdstr("AlternateSavePath"));
        cursor.EmitLdstr("Modded");
        cursor.EmitStloc0();
    }
}
