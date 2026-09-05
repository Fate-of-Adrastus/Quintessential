#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Cil;
using MonoMod.InlineRT;
using Quintessential;
using System;
using System.Collections.Generic;
using System.Linq;

public class patch_Sim : Sim {

    public RecipeInputDictionary<HexIndex, IRecipeInput> RecipeInputs;
    public RecipeOutputDictionary<HexIndex, IRecipeOutput> RecipeOutputs;
    public List<Part> HoldingParts;

    private void InitRecileDictionaries() {
        this.RecipeInputs = new(this);
        this.RecipeOutputs = new(this);
    }

    public bool GetAtomReference(Part part, HexIndex offset, bool allowPartAttachedAtoms, out AtomReference atomReference) {
        return GetAtomReference(part, offset, HoldingParts, allowPartAttachedAtoms).GetOrDefault(out atomReference);
    }
    public bool HasAtomAt(Part part, HexIndex offset, bool allowPartAttachedAtoms) {
        return GetAtomReference(part, offset, HoldingParts, allowPartAttachedAtoms).HasValue();
    }

    [MonoModILInject(".ctor")]
    static void PatchCtor(MethodDefinition method, CustomAttribute attribute) {

        MonoModRule.Modder.Log("Patching Sim init.");
        if (!method.HasBody) {
            throw new Exception("Unable to patch Sim init. (no body)");
        }
        ILCursor cursor = new(new ILContext(method));
        MethodReference init = MonoModRule.Modder.FindType("Sim").Resolve().Methods.First(f => f.Name.Equals("InitRecileDictionaries"));
        cursor.EmitLdarg0();
        cursor.EmitCall(init);
    }

    [MonoModILInject("RunCycleGlyphs")]
    static void PatchRecipeSystem(MethodDefinition method, CustomAttribute attribute) {

        MonoModRule.Modder.Log("Patching Recipe System init.");
        if (!method.HasBody) {
            throw new Exception("Unable to patch Recipe System init. (no body)");
        }
        ILCursor cursor = new(new ILContext(method));


        // Replace loc0 with field
        FieldDefinition holdingParts = MonoModRule.Modder.FindType("Sim").Resolve().Fields.First(f => f.Name.Equals("HoldingParts"));
        cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallvirt("Sim", "SpawnMolecules"));
        cursor.EmitLdarg0();
        cursor.EmitLdloc0();
        cursor.EmitStfld(holdingParts);

        if (!cursor.TryGotoNext(MoveType.Before,
            instr => instr.MatchLdarg0(),
            instr => instr.MatchCallvirt("Sim","GetSolution"),
            instr => instr.MatchLdfld("Solution", "parts"),
            instr => instr.OpCode == OpCodes.Callvirt,
            instr => instr.MatchStloc2(),
            instr => instr.OpCode == OpCodes.Br
        )) {
            throw new Exception("Unable to patch Recipe System init. (no target)");
        }
        Instruction loopEnd = null;
        while (cursor.Prev.OpCode != OpCodes.Br) {
            cursor.Index++;
            if (loopEnd == null && cursor.Prev.OpCode == OpCodes.Br) loopEnd = (Instruction)cursor.Prev.Operand;
        }
        TypeDefinition recipeType = MonoModRule.Modder.FindType("Quintessential.GlyphRecipe").Resolve();
        FieldDefinition recipesField = recipeType.Fields.First(f => f.Name.Equals("GlyphRecipes"));

        // Get the reference for the code
        MethodDefinition referenceCode = MonoModRule.Modder.FindType("Sim").Resolve().Methods.First(f => f.Name.Equals("PatchRecipeSystemCodeReference"));
        ILCursor referenceCulsor = new(new ILContext(referenceCode));
        TypeReference enumeratorType = referenceCode.Body.Variables[0].VariableType;
        MethodReference getEnumeratior = null;
        MethodReference moveNextEnumeratior = null;
        TypeReference currentValueType = referenceCode.Body.Variables[1].VariableType;
        MethodReference getCurrent = null;
        referenceCulsor.TryGotoNext(instr => instr.MatchCallvirt(out getEnumeratior));
        referenceCulsor.TryGotoNext(instr => instr.MatchCall(out moveNextEnumeratior));
        referenceCulsor.TryGotoNext(instr => instr.MatchCall(out getCurrent));
        cursor.TryGotoNext(MoveType.Before,
            instr => instr.MatchLdloc(6),
            instr => instr.MatchLdfld("Sim/ReferredPart", "part"),
            instr => instr.MatchCallvirt("Part", "GetType"),
            instr => instr.MatchLdsfld("PartTypes", "calcificationGlyph"));

        // --- Create new local eumerator & start creating the loop
        method.Body.Variables.Add(new VariableDefinition(enumeratorType));
        var enumeratorVar = method.Body.Variables[^1];

        cursor.EmitLdsfld(recipesField);
        var first = cursor.Prev;
        cursor.EmitCallvirt(getEnumeratior);
        cursor.EmitStloc(enumeratorVar);
        // We should cursor.EmitBr(); but first get the target instruction Set
        var upperHead = cursor.Prev;
        cursor.Goto(loopEnd);

        Instruction continueTarget = (Instruction)cursor.Next.Next.Next.Operand; // BrtrueS
        //var dispose = (MethodReference)cursor.Next.Next.Next.Next.Next.Next.Next.Operand; // call virt

        cursor.EmitLdloca(enumeratorVar);
        Instruction last = cursor.Prev;
        int lastIndex = cursor.Index;
        cursor.EmitCall(moveNextEnumeratior);
        cursor.Emit(OpCodes.Brtrue, continueTarget); // Target willbe set later
        var breakTarget = cursor.Next; // TODO must fix after adding finally block
        //  TODO get the finally block working, does that bastard allways throw unfixable errors?
        //  Hopefully commenting this out won't blow up anyones computer...
        //cursor.Emit(OpCodes.Leave, loopEnd);
        //cursor.EmitLdloca(enumeratorVar);
        //var tryEnd_handleStart = cursor.Prev;
        //cursor.EmitConstrained(enumeratorType);
        //cursor.EmitCallvirt(dispose.Resolve());
        //cursor.EmitEndfinally();
        //var handleEnd = cursor.Next;

        cursor.Goto(upperHead, MoveType.After); // Add Br to start
        cursor.Emit(OpCodes.Br, last);
        //var tryStart = cursor.Prev;
        cursor.TryGotoNext(MoveType.Before, instr => instr.OpCode == OpCodes.Brfalse && instr.Operand == loopEnd); // Fix a problem in the middle of the iteration
        cursor.Next.Operand = last;

        cursor.Goto(upperHead, MoveType.After); // Finished with the loop creation
        cursor.Index++;

        //cursor.Goto(upperHead, MoveType.After);   // use to clear the entire loop if needed
        //cursor.RemoveRange(lastIndex - cursor.Index + 3);

        //method.Body.ExceptionHandlers.Insert(1, new(ExceptionHandlerType.Finally) {
        //    TryStart = tryStart,
        //    TryEnd = tryEnd_handleStart,
        //    HandlerStart = tryEnd_handleStart,
        //    HandlerEnd = handleEnd
        //});

        method.Body.Variables.Add(new VariableDefinition(currentValueType)); // Add local for varriable
        var recipePairVar = method.Body.Variables[^1];
        cursor.EmitLdloca(enumeratorVar);
        var newBegining = cursor.Prev;
        cursor.EmitCall(getCurrent);
        cursor.EmitStloc(recipePairVar);
        upperHead = cursor.Prev;

        cursor.Goto(loopEnd); // Change the start of the main loop to include the new instructions
        cursor.TryGotoPrev(MoveType.Before, instr => instr.OpCode == OpCodes.Brtrue);
        cursor.Next.Operand = newBegining;
        cursor.Goto(upperHead, MoveType.After);

        // --- Add break conditions
        cursor.EmitLdloc(7);
        cursor.EmitLdfld(MonoModRule.Modder.FindType("PartSimState").Resolve().Fields.First(f => f.Name.Equals("wasActivated")));
        cursor.Emit(OpCodes.Brtrue, breakTarget); // break;

        // --- Add condition for recipe
        cursor.EmitLdloc(6);
        upperHead = cursor.Prev;
        cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld("Sim/ReferredPart", "part"), instr => instr.MatchCallvirt("Part", "GetType"));
        FieldReference referencePart = (FieldReference)cursor.Previous.Previous.Operand;
        MethodReference getPartType = (MethodReference)cursor.Previous.Operand;
        cursor.Goto(upperHead, MoveType.After);
        cursor.EmitLdfld(referencePart);
        cursor.EmitCallvirt(getPartType);
        MethodDefinition partTypeId = MonoModRule.Modder.FindType("PartType").Resolve().Methods.First(f => f.Name.Equals("get_Id"));
        cursor.EmitCall(partTypeId);

        MethodReference getKey = null;
        referenceCulsor.TryGotoNext(instr => instr.MatchCall(out getKey));
        cursor.EmitLdloc(recipePairVar);
        cursor.EmitCall(getKey);

        MethodReference compareId = null;
        referenceCulsor.TryGotoNext(instr => instr.MatchCall(out compareId));
        cursor.EmitCall(compareId);
        cursor.Emit(OpCodes.Brfalse, last);
        upperHead = cursor.Prev;

        // --- Add separate variable
        MethodReference getValue = null;
        referenceCulsor.TryGotoNext(instr => instr.MatchCall(out getValue));
        method.Body.Variables.Add(new VariableDefinition(recipeType)); // Add local for varriable
        var recipeVar = method.Body.Variables[^1];
        cursor.EmitLdloc(recipePairVar);
        cursor.EmitCall(getValue);
        cursor.EmitStloc(recipeVar);

        // --- Replace Vanilla Recipe Calls
        FieldReference predicate = null;
        referenceCulsor.TryGotoNext(instr => instr.MatchLdfld(out predicate));
        MethodReference invokeAndClear = null;
        referenceCulsor.TryGotoNext(instr => instr.MatchCall(out invokeAndClear));
        FieldReference inputs = null;
        referenceCulsor.TryGotoNext(instr => instr.MatchLdfld(out inputs));
        FieldReference outputs = null;
        referenceCulsor.TryGotoNext(instr => instr.MatchLdfld(out outputs));
        MethodReference hexCtor = null;
        referenceCulsor.TryGotoNext(instr => instr.MatchNewobj(out hexCtor));
        MethodReference getIRecipeIO = null;
        referenceCulsor.TryGotoNext(instr => instr.MatchCallvirt(out getIRecipeIO));
        TypeReference atomRef = null;
        referenceCulsor.TryGotoNext(instr => instr.MatchIsinst(out atomRef));
        FieldReference processingAtoms = null;
        referenceCulsor.TryGotoNext(instr => instr.MatchLdfld(out processingAtoms));
        FieldReference dictionaryRecipe = null;
        referenceCulsor.TryGotoNext(instr => instr.MatchStfld(out dictionaryRecipe));
        FieldReference atomTags = null;
        referenceCulsor.TryGotoNext(instr => instr.MatchLdsfld(out atomTags));
        MethodReference stringToId = null;
        referenceCulsor.TryGotoNext(instr => instr.MatchCall(out stringToId));
        MethodReference getTag = null;
        referenceCulsor.TryGotoNext(instr => instr.MatchCallvirt(out getTag));
        MethodReference hasAtomTag = null;
        referenceCulsor.TryGotoNext(instr => instr.MatchCallvirt(out hasAtomTag));
        TypeDefinition atomType = MonoModRule.Modder.FindType("AtomType").Resolve();
        method.Body.Variables.Add(new VariableDefinition(atomType));
        VariableDefinition oldAtomType = method.Body.Variables[^1];

        // --- Set recipe for IO dictionaries
        cursor.EmitLdarg0();
        cursor.EmitLdfld(inputs);
        cursor.EmitLdloc(recipeVar);
        cursor.EmitStfld(dictionaryRecipe);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(outputs);
        cursor.EmitLdloc(recipeVar);
        cursor.EmitStfld(dictionaryRecipe);

        // Calcification
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCallvirt("Sim","GetAtomReference"));
        cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld("AtomType", "isGlassy"));
        var removeIndex = cursor.Index;
        cursor.TryGotoPrev(MoveType.Before, instr => instr.MatchLdarg0());
        cursor.RemoveRange(removeIndex - cursor.Index);
        cursor.EmitLdloc(recipeVar);
        cursor.EmitLdfld(predicate);
        cursor.EmitLdarg0(); // Sim
        cursor.EmitLdloc(6); // ReferencePart::part
        cursor.EmitLdfld(referencePart);
        cursor.EmitCall(invokeAndClear);
        cursor.Index++;
        cursor.EmitLdarg0();
        cursor.EmitLdfld(inputs);
        cursor.EmitLdcI4(0);
        cursor.EmitLdcI4(0);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomRef);
        cursor.EmitStloc(8);
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdsfld("AtomTypes", "salt"));
        cursor.Remove();
        cursor.EmitLdarg0();
        cursor.EmitLdfld(outputs);
        cursor.EmitLdcI4(0);
        cursor.EmitLdcI4(0);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomType);

        // Duplication
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCallvirt("Sim", "GetAtomReference"));
        cursor.TryGotoPrev(MoveType.After, instr => instr.MatchLdarg0());
        var start = cursor.Prev;
        cursor.TryGotoNext(MoveType.After, instr => instr.MatchCall("AtomType", "op_Equality"));
        removeIndex = cursor.Index;
        cursor.Goto(start);
        cursor.RemoveRange(removeIndex - cursor.Index);
        cursor.EmitLdloc(recipeVar);
        cursor.EmitLdfld(predicate);
        cursor.EmitLdarg0(); // Sim
        cursor.EmitLdloc(6); // ReferencePart::part
        cursor.EmitLdfld(referencePart);
        cursor.EmitCall(invokeAndClear);
        cursor.Index++;
        cursor.EmitLdarg0();
        cursor.EmitLdfld(inputs);
        cursor.EmitLdcI4(0);
        cursor.EmitLdcI4(0);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomRef);
        cursor.EmitStloc(10);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(inputs);
        cursor.EmitLdcI4(1);
        cursor.EmitLdcI4(0);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomRef);
        cursor.EmitStloc(11);
        start = cursor.Prev;
        cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld("AtomReference", "atomType"));
        cursor.Prev.MatchLdfld(out FieldReference atomRefAtomType);
        cursor.Goto(start, MoveType.After);
        cursor.EmitLdloc(11);
        cursor.EmitLdfld(atomRefAtomType);
        cursor.EmitStloc(oldAtomType);
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdsfld("AtomTypes", "salt"));
        cursor.Remove();
        cursor.EmitLdloc(oldAtomType);

        // Projection
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCallvirt("Sim", "GetAtomReference"));
        cursor.TryGotoPrev(MoveType.After, instr => instr.MatchLdarg0());
        start = cursor.Prev;
        cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld("AtomReference", "isHeldByArm"));
        removeIndex = cursor.Index;
        cursor.Goto(start);
        cursor.RemoveRange(removeIndex - cursor.Index);
        cursor.EmitLdloc(recipeVar);
        cursor.EmitLdfld(predicate);
        cursor.EmitLdarg0(); // Sim
        cursor.EmitLdloc(6); // ReferencePart::part
        cursor.EmitLdfld(referencePart);
        cursor.EmitCall(invokeAndClear);
        if (cursor.Next.OpCode == OpCodes.Brtrue) cursor.Next.OpCode = OpCodes.Brfalse;
        cursor.Index++;
        cursor.EmitLdarg0();
        cursor.EmitLdfld(inputs);
        cursor.EmitLdcI4(1);
        cursor.EmitLdcI4(0);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomRef);
        cursor.EmitStloc(15);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(inputs);
        cursor.EmitLdcI4(0);
        cursor.EmitLdcI4(0);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomRef);
        cursor.EmitStloc(16);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(outputs);
        cursor.EmitLdcI4(0);
        cursor.EmitLdcI4(0);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomType);
        cursor.EmitStloc(17);
        cursor.EmitLdloc(7);
        cursor.EmitLdcI4(1);
        cursor.EmitNewarr(atomType);
        cursor.EmitDup();
        cursor.EmitLdcI4(0);
        cursor.EmitLdloc(16);
        cursor.EmitLdfld(atomRefAtomType);
        cursor.EmitStelemRef();
        cursor.EmitStfld(processingAtoms);

        // Rejection
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCallvirt("Sim", "GetAtomReference"));
        cursor.TryGotoPrev(MoveType.After, instr => instr.MatchLdarg0());
        start = cursor.Prev;
        cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdloca(22));
        cursor.Index++;
        removeIndex = cursor.Index;
        cursor.Goto(start);
        cursor.RemoveRange(removeIndex - cursor.Index);
        cursor.EmitLdloc(recipeVar);
        cursor.EmitLdfld(predicate);
        cursor.EmitLdarg0(); // Sim
        cursor.EmitLdloc(6); // ReferencePart::part
        cursor.EmitLdfld(referencePart);
        cursor.EmitCall(invokeAndClear);
        cursor.Index++;
        cursor.EmitLdarg0();
        cursor.EmitLdfld(inputs);
        cursor.EmitLdloc(19);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomRef);
        cursor.EmitStloc(21);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(outputs);
        cursor.EmitLdloc(19);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomType);
        cursor.EmitStloc(22);
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdsfld("AtomTypes", "quicksilver"));
        cursor.Remove();
        cursor.EmitLdarg0();
        cursor.EmitLdfld(outputs);
        cursor.EmitLdloc(20);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomType);
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdsfld("AtomTypes", "quicksilver"));
        cursor.Remove();
        cursor.EmitLdarg0();
        cursor.EmitLdfld(outputs);
        cursor.EmitLdloc(20);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomType);

        // Purification
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCallvirt("Sim", "GetAtomReference"));
        cursor.TryGotoPrev(MoveType.After, instr => instr.MatchLdarg0());
        start = cursor.Prev;
        cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld("AtomReference", "isHeldByArm"));
        cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld("AtomReference", "isHeldByArm"));
        removeIndex = cursor.Index;
        cursor.Goto(start);
        cursor.RemoveRange(removeIndex - cursor.Index);
        cursor.EmitLdloc(recipeVar);
        cursor.EmitLdfld(predicate);
        cursor.EmitLdarg0(); // Sim
        cursor.EmitLdloc(6); // ReferencePart::part
        cursor.EmitLdfld(referencePart);
        cursor.EmitCall(invokeAndClear);
        if (cursor.Next.OpCode == OpCodes.Brtrue) cursor.Next.OpCode = OpCodes.Brfalse;
        cursor.Index++;
        cursor.EmitLdarg0();
        cursor.EmitLdfld(inputs);
        cursor.EmitLdcI4(0);
        cursor.EmitLdcI4(0);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomRef);
        cursor.EmitStloc(28);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(inputs);
        cursor.EmitLdcI4(1);
        cursor.EmitLdcI4(0);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomRef);
        cursor.EmitStloc(29);
        cursor.RemoveRange(2);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(outputs);
        cursor.EmitLdloc(27);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomType);
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdflda("AtomType", "successorMetal"));
        cursor.RemoveRange(2);

        // Division
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCallvirt("Sim", "GetAtomReference"));
        cursor.TryGotoPrev(MoveType.After, instr => instr.MatchLdarg0());
        start = cursor.Prev;
        cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld("AtomReference", "isHeldByArm"));
        removeIndex = cursor.Index;
        cursor.Goto(start);
        cursor.RemoveRange(removeIndex - cursor.Index);
        cursor.EmitLdloc(recipeVar);
        cursor.EmitLdfld(predicate);
        cursor.EmitLdarg0(); // Sim
        cursor.EmitLdloc(6); // ReferencePart::part
        cursor.EmitLdfld(referencePart);
        cursor.EmitCall(invokeAndClear);
        if (cursor.Next.OpCode == OpCodes.Brtrue) cursor.Next.OpCode = OpCodes.Brfalse;
        cursor.Index++;
        cursor.EmitLdarg0();
        cursor.EmitLdfld(inputs);
        cursor.EmitLdcI4(0);
        cursor.EmitLdcI4(0);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomRef);
        cursor.EmitStloc(39);
        cursor.GotoNext(MoveType.Before, instr => instr.MatchLdloc(40));
        while (!cursor.Next.MatchStelemRef()) cursor.Remove();
        cursor.EmitLdarg0();
        cursor.EmitLdfld(outputs);
        cursor.EmitLdcI4(-1);
        cursor.EmitLdcI4(0);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomType);
        cursor.GotoNext(MoveType.Before, instr => instr.MatchLdloc(40));
        while (!cursor.Next.MatchStelemRef()) cursor.Remove();
        cursor.EmitLdarg0();
        cursor.EmitLdfld(outputs);
        cursor.EmitLdcI4(1);
        cursor.EmitLdcI4(0);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomType);

        // Proliferation
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCallvirt("Sim", "GetAtomReference"));
        cursor.TryGotoPrev(MoveType.After, instr => instr.MatchLdarg0());
        start = cursor.Prev;
        cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld("AtomReference", "isHeldByArm"));
        removeIndex = cursor.Index;
        cursor.Goto(start);
        cursor.RemoveRange(removeIndex - cursor.Index);
        cursor.EmitLdloc(recipeVar);
        cursor.EmitLdfld(predicate);
        cursor.EmitLdarg0(); // Sim
        cursor.EmitLdloc(6); // ReferencePart::part
        cursor.EmitLdfld(referencePart);
        cursor.EmitCall(invokeAndClear);
        if (cursor.Next.OpCode == OpCodes.Brtrue) cursor.Next.OpCode = OpCodes.Brfalse;
        cursor.Index++;
        cursor.EmitLdarg0();
        cursor.EmitLdfld(inputs);
        cursor.EmitLdloc(47);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomRef);
        cursor.EmitStloc(50);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(inputs);
        cursor.EmitLdloc(49);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomRef);
        cursor.EmitStloc(51);
        cursor.TryGotoNext(MoveType.After, instr => instr.MatchStelemRef());
        cursor.EmitDup();
        cursor.Emit(OpCodes.Ldc_I4_1);
        cursor.EmitLdloc(50);
        cursor.EmitLdfld(atomRefAtomType);
        cursor.EmitStelemRef();
        cursor.GotoPrev(MoveType.Before, instr => instr.OpCode == OpCodes.Ldc_I4_1);
        cursor.GotoPrev(MoveType.Before, instr => instr.OpCode == OpCodes.Ldc_I4_1);
        cursor.Remove();
        cursor.Emit(OpCodes.Ldc_I4_2);

        // Animismus
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCallvirt("Sim", "GetAtomReference"));
        cursor.TryGotoPrev(MoveType.After, instr => instr.MatchLdarg0());
        start = cursor.Prev;
        cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld("AtomReference", "isHeldByArm"));
        cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld("AtomReference", "isHeldByArm"));
        removeIndex = cursor.Index;
        cursor.Goto(start);
        cursor.RemoveRange(removeIndex - cursor.Index);
        cursor.EmitLdloc(recipeVar);
        cursor.EmitLdfld(predicate);
        cursor.EmitLdarg0(); // Sim
        cursor.EmitLdloc(6); // ReferencePart::part
        cursor.EmitLdfld(referencePart);
        cursor.EmitCall(invokeAndClear);
        if (cursor.Next.OpCode == OpCodes.Brtrue) cursor.Next.OpCode = OpCodes.Brfalse;
        cursor.Index++;
        cursor.EmitLdarg0();
        cursor.EmitLdfld(inputs);
        cursor.EmitLdcI4(0);
        cursor.EmitLdcI4(0);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomRef);
        cursor.EmitStloc(59);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(inputs);
        cursor.EmitLdcI4(1);
        cursor.EmitLdcI4(0);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomRef);
        cursor.EmitStloc(60);
        cursor.EmitLdloc(7);
        cursor.EmitLdcI4(4);
        cursor.EmitNewarr(atomType);
        cursor.EmitDup();
        cursor.EmitLdcI4(0);
        cursor.EmitLdloc(59);
        cursor.EmitLdfld(atomRefAtomType);
        cursor.EmitStelemRef();
        cursor.EmitDup();
        cursor.EmitLdcI4(1);
        cursor.EmitLdloc(60);
        cursor.EmitLdfld(atomRefAtomType);
        cursor.EmitStelemRef();
        cursor.EmitDup();
        cursor.EmitLdcI4(2);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(outputs);
        cursor.EmitLdloc(57);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomType);
        cursor.EmitStelemRef();
        cursor.EmitDup();
        cursor.EmitLdcI4(3);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(outputs);
        cursor.EmitLdloc(58);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomType);
        cursor.EmitStelemRef();
        cursor.EmitStfld(processingAtoms);
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdsfld("AtomTypes", "vitae"));
        cursor.Remove();
        cursor.EmitLdloc(7);
        cursor.EmitLdfld(processingAtoms);
        cursor.EmitLdcI4(2);
        cursor.EmitLdelemRef();
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdsfld("AtomTypes", "mors"));
        cursor.Remove();
        cursor.EmitLdloc(7);
        cursor.EmitLdfld(processingAtoms);
        cursor.EmitLdcI4(3);
        cursor.EmitLdelemRef();

        // Unification
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCallvirt("Sim", "GetAtomReference"));
        cursor.TryGotoPrev(MoveType.After, instr => instr.MatchLdarg0());
        start = cursor.Prev;
        cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdftn("Sim/LambdaGeneratedClass", "IsAir"));
        cursor.TryGotoNext(MoveType.After, instr => instr.OpCode == OpCodes.Bne_Un);
        var oldTarget = (Instruction)cursor.Prev.Operand;
        removeIndex = cursor.Index;
        cursor.Goto(start);
        cursor.RemoveRange(removeIndex - cursor.Index);
        cursor.EmitLdloc(recipeVar);
        cursor.EmitLdfld(predicate);
        cursor.EmitLdarg0(); // Sim
        cursor.EmitLdloc(6); // ReferencePart::part
        cursor.EmitLdfld(referencePart);
        cursor.EmitCall(invokeAndClear);
        cursor.Emit(OpCodes.Brfalse, oldTarget);
        cursor.EmitLdcI4(4);
        cursor.EmitNewarr(atomRef);
        cursor.EmitDup();
        cursor.EmitLdcI4(0);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(inputs);
        cursor.EmitLdcI4(-1);
        cursor.EmitLdcI4(1);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomRef);
        cursor.EmitStelemRef();
        cursor.EmitDup();
        cursor.EmitLdcI4(1);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(inputs);
        cursor.EmitLdcI4(0);
        cursor.EmitLdcI4(1);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomRef);
        cursor.EmitStelemRef();
        cursor.EmitDup();
        cursor.EmitLdcI4(2);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(inputs);
        cursor.EmitLdcI4(0);
        cursor.EmitLdcI4(-1);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomRef);
        cursor.EmitStelemRef();
        cursor.EmitDup();
        cursor.EmitLdcI4(3);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(inputs);
        cursor.EmitLdcI4(1);
        cursor.EmitLdcI4(-1);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomRef);
        cursor.EmitStelemRef();
        cursor.EmitStloc(67);
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdftn("Sim/LambdaGeneratedClass", "GetType"));
        cursor.TryGotoPrev(MoveType.After, instr => instr.MatchLdloc(67));
        start = cursor.Prev;
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchStfld("PartSimState", "processingAtoms"));
        removeIndex = cursor.Index;
        cursor.Goto(start);
        cursor.RemoveRange(removeIndex - cursor.Index);
        cursor.EmitLdcI4(5);
        cursor.EmitNewarr(atomType);
        cursor.EmitDup();
        cursor.EmitLdcI4(0);
        cursor.EmitLdloc(67);
        cursor.EmitLdcI4(0);
        cursor.EmitLdelemRef();
        cursor.EmitLdfld(atomRefAtomType);
        cursor.EmitStelemRef();
        cursor.EmitDup();
        cursor.EmitLdcI4(1);
        cursor.EmitLdloc(67);
        cursor.EmitLdcI4(1);
        cursor.EmitLdelemRef();
        cursor.EmitLdfld(atomRefAtomType);
        cursor.EmitStelemRef();
        cursor.EmitDup();
        cursor.EmitLdcI4(2);
        cursor.EmitLdloc(67);
        cursor.EmitLdcI4(2);
        cursor.EmitLdelemRef();
        cursor.EmitLdfld(atomRefAtomType);
        cursor.EmitStelemRef();
        cursor.EmitDup();
        cursor.EmitLdcI4(3);
        cursor.EmitLdloc(67);
        cursor.EmitLdcI4(3);
        cursor.EmitLdelemRef();
        cursor.EmitLdfld(atomRefAtomType);
        cursor.EmitStelemRef();
        cursor.EmitDup();
        cursor.EmitLdcI4(4);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(outputs);
        cursor.EmitLdcI4(0);
        cursor.EmitLdcI4(0);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomType);
        cursor.EmitStelemRef();
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdsfld("AtomTypes", "quintessence"));
        cursor.Remove();
        cursor.EmitLdloc(7);
        cursor.EmitLdfld(processingAtoms);
        cursor.EmitLdcI4(4);
        cursor.EmitLdelemRef();

        // Dispersion
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCallvirt("Sim", "GetAtomReference"));
        cursor.TryGotoPrev(MoveType.After, instr => instr.MatchLdarg0());
        start = cursor.Prev;
        cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld("AtomReference", "isHeldByArm"));
        removeIndex = cursor.Index;
        cursor.Goto(start);
        cursor.RemoveRange(removeIndex - cursor.Index);
        cursor.EmitLdloc(recipeVar);
        cursor.EmitLdfld(predicate);
        cursor.EmitLdarg0(); // Sim
        cursor.EmitLdloc(6); // ReferencePart::part
        cursor.EmitLdfld(referencePart);
        cursor.EmitCall(invokeAndClear);
        if (cursor.Next.OpCode == OpCodes.Brtrue) cursor.Next.OpCode = OpCodes.Brfalse;
        cursor.Index++;
        cursor.EmitLdarg0();
        cursor.EmitLdfld(inputs);
        cursor.EmitLdcI4(0);
        cursor.EmitLdcI4(0);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomRef);
        cursor.EmitStloc(71);
        cursor.EmitLdloc(7);
        cursor.EmitLdcI4(5);
        cursor.EmitNewarr(atomType);
        cursor.EmitDup();
        cursor.EmitLdcI4(0);
        cursor.EmitLdloc(71);
        cursor.EmitLdfld(atomRefAtomType);
        cursor.EmitStelemRef();
        cursor.EmitDup();
        cursor.EmitLdcI4(1);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(outputs);
        cursor.EmitLdcI4(-1);
        cursor.EmitLdcI4(0);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomType);
        cursor.EmitStelemRef();
        cursor.EmitDup();
        cursor.EmitLdcI4(2);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(outputs);
        cursor.EmitLdcI4(0);
        cursor.EmitLdcI4(-1);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomType);
        cursor.EmitStelemRef();
        cursor.EmitDup();
        cursor.EmitLdcI4(3);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(outputs);
        cursor.EmitLdcI4(1);
        cursor.EmitLdcI4(-1);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomType);
        cursor.EmitStelemRef();
        cursor.EmitDup();
        cursor.EmitLdcI4(4);
        cursor.EmitLdarg0();
        cursor.EmitLdfld(outputs);
        cursor.EmitLdcI4(1);
        cursor.EmitLdcI4(0);
        cursor.EmitNewobj(hexCtor);
        cursor.EmitCallvirt(getIRecipeIO);
        cursor.EmitIsinst(atomType);
        cursor.EmitStelemRef();
        cursor.EmitStfld(processingAtoms);
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdsfld("AtomTypes", "air"));
        cursor.Remove();
        cursor.EmitLdloc(7);
        cursor.EmitLdfld(processingAtoms);
        cursor.EmitLdcI4(1);
        cursor.EmitLdelemRef();
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdsfld("AtomTypes", "fire"));
        cursor.Remove();
        cursor.EmitLdloc(7);
        cursor.EmitLdfld(processingAtoms);
        cursor.EmitLdcI4(2);
        cursor.EmitLdelemRef();
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdsfld("AtomTypes", "water"));
        cursor.Remove();
        cursor.EmitLdloc(7);
        cursor.EmitLdfld(processingAtoms);
        cursor.EmitLdcI4(3);
        cursor.EmitLdelemRef();
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdsfld("AtomTypes", "earth"));
        cursor.Remove();
        cursor.EmitLdloc(7);
        cursor.EmitLdfld(processingAtoms);
        cursor.EmitLdcI4(4);
        cursor.EmitLdelemRef();

        // Disposal
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCallvirt("Sim", "GetAtomReference"));
        cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld("AtomReference", "isHeldByArm"));
        cursor.TryGotoNext(MoveType.After, instr => instr.OpCode == OpCodes.Brtrue);
        var target = (Instruction)cursor.Prev.Operand;
        cursor.EmitLdsfld(atomTags);
        cursor.EmitLdstr("om:indisposable");
        cursor.EmitCall(stringToId);
        cursor.EmitCallvirt(getTag);
        cursor.EmitLdloc(76);
        cursor.EmitCallvirt(hasAtomTag);
        cursor.Emit(OpCodes.Brtrue, target);

        referenceCode.DeclaringType.Methods.Remove(referenceCode);
    }
    private void PatchRecipeSystemCodeReference(Sim sim, PartSimState simState) { // this method is oly used as a source to copy relevant IL code from
        var enumerator = GlyphRecipe.GlyphRecipes.GetEnumerator();
        enumerator.MoveNext();
        var current = enumerator.Current;
        var key = GetMatchingGlyphId(current);
        #pragma warning disable CS1718 // Comparison made to same variable
        var equality = key == key;
        #pragma warning restore CS1718 // Comparison made to same variable
        GetValue(current).Predicate.InvokeAndClear(null, null);
        var @in = RecipeInputs;
        var atomRefOut = RecipeOutputs[new HexIndex(0, 0)] as AtomReference;
        var procAtoms = simState.processingAtoms;
        @in.recipe = null;
        AtomTag.AtomTags["om:calcifiable"].HasAtom(atomRefOut);
    }

    private static Identifier GetMatchingGlyphId(KeyValuePair<Identifier, GlyphRecipe> pair) => pair.Value.RecipeGlyphId; // Workaround for the weirdest internal CLR error ever
    private static GlyphRecipe GetValue(KeyValuePair<Identifier, GlyphRecipe> pair) => pair.Value; // Workaround for the weirdest internal CLR error ever
}
