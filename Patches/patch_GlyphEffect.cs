using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Cil;
using MonoMod.InlineRT;
using System;
using System.Linq;

#pragma warning disable IDE1006 // Naming Styles

[MonoModPatch("GlyphEffect")]
public class patch_GlyphEffect
{
    public Color Color;

    public patch_GlyphEffect WithColor(Color color)
    {
        Color = color;
        return this;
    }

    [MonoModILInject(".ctor")]
    public static void PatchGlyphEffectConstructor(MethodDefinition method, CustomAttribute attrib) {
        MonoModRule.Modder.Log("Patching glyph effect (1/2)");

        if (!method.HasBody) {
            Console.WriteLine("Unable to patch glyph effect constructor (no body)");
            throw new Exception();
        }

        ILCursor gremlin = new(new ILContext(method));

        if (!gremlin.TryGotoNext(MoveType.Before,
            instr => instr.MatchRet()
        )) {
            Console.WriteLine("Unable to patch glyph effect constructor (no return)");
            throw new Exception();
        }

        TypeDefinition holder = MonoModRule.Modder.FindType("GlyphEffect").Resolve();
        FieldDefinition colorProp = holder.Fields.First((f) => f.Name == "Color");

        holder = MonoModRule.Modder.FindType("Color").Resolve();
        FieldDefinition colorWhite = holder.Fields.First((f) => f.IsStatic && f.Name == "White");


        gremlin.Emit(OpCodes.Ldarg_0);
        gremlin.Emit(OpCodes.Ldsfld, colorWhite);
        gremlin.Emit(OpCodes.Stfld, colorProp);

    }
}