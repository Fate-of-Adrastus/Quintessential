using Mono.Cecil;
using MonoMod;
using Quintessential;
using System.Collections.Generic;

#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

abstract class patch_SolutionEditorBase : SolutionEditorBase
{

    // renders parts
    // adds support for custom part renderers

    public extern void orig_RenderPartBase(Part part, Vector2 pos);
    public void RenderPartBase(Part part, Vector2 pos)
    {
        orig_RenderPartBase(part, pos);
        IntermediatePartState class195 = GetIntermState(part, pos);
        PartRenderer renderer = new(class195.pos, class195.rotation, Editor.method_922());
        foreach (var r in QApi.PartRenderers)
            if (r.Left(part))
                r.Right(part, pos, this, renderer);
    }


    [MonoModILInject("method_2451")]
    public static void PatchGlyphEffectRenderer(MethodDefinition method, CustomAttribute attrib) {
        // TODO: Reworke this to match new version changes
        return;
        //MonoModRule.Modder.Log("Patching glyph effect (2/2)");

        //if (!method.HasBody)
        //{
        //    Console.WriteLine("Unable to patch glyph effect renderer (no body)");
        //    throw new Exception();
        //}

        //ILCursor gremlin = new(new ILContext(method));

        //TypeDefinition holder = MonoModRule.Modder.FindType("Color").Resolve();
        //FieldDefinition colorWhite = holder.Fields.First((f) => f.IsStatic && f.Name == "White");




        //if (!gremlin.TryGotoNext(MoveType.Before,
        //    instr =>
        //    {
        //        FieldReference testOperand = instr.Operand as FieldReference;
        //        return instr.OpCode == OpCodes.Ldsfld && testOperand.FieldType == colorWhite.FieldType && testOperand.Name == colorWhite.Name;
        //    },
        //    instr => instr.MatchLdloc(6),
        //    instr => instr.OpCode == OpCodes.Call
        //))
        //{
        //    Console.WriteLine("Unable to patch glyph effect renderer (no draw call)");
        //    throw new Exception();
        //}

        //holder = MonoModRule.Modder.FindType("GlyphEffect").Resolve();
        //FieldDefinition colorProp = holder.Fields.First((f) => f.Name == "Color");

        //gremlin.Remove();
        //gremlin.Emit(OpCodes.Ldloc, 1);
        //gremlin.Emit(OpCodes.Ldfld, colorProp);
    }
}