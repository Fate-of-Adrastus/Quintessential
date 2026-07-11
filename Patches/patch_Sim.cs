using System.Collections.Generic;
using System.Linq;
using MonoMod;
using Quintessential;

#pragma warning disable CS0649 // Field is never assigned to
#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

class patch_Sim{

	// Make important fields public
	[MonoModPublic]
	public SolutionEditorBase solutionEditor;
	[MonoModPublic]
	public Dictionary<Part, PartSimState> simulationDict;
	[MonoModPublic]
	public Dictionary<Part, Sim.HolderMovementInfo> holderInfos;
	[MonoModPublic]
	public List<Molecule> molecules;
	[MonoModPublic]
	public List<Sim.Collider> additionalCollisions;
	
	// Hold onto held grippers
	public List<Part> HeldGrippers;

	// Helper methods to find held or unheld atoms
	public Maybe<AtomReference> FindAtomRelative(Part part, HexIndex offset){
		return FindAtom(part.InFrontBy(offset));
	}
	
	public Maybe<AtomReference> FindAtom(HexIndex position){
		var simStates = simulationDict;
		foreach(Molecule molecule in molecules) {
			if(molecule.GetAtoms().TryGetValue(position, out Atom atom)){
				bool isHeld = HeldGrippers != null && HeldGrippers.Any(part => simStates[part].newPosition == position);
				return new AtomReference(molecule, position, atom.atomType, atom, isHeld);
			}
		}

		return MaybeHelper.empty;
	}

    // Run custom behaviours
    public extern void orig_RunCycleGlyphs(bool isCycleStart);
    //public extern void RunCycleGlyphsMain(bool isCycleStart);
	//[MonoModReplace]
	public void RunCycleGlyphs(bool isCycleStart) {
		// fill the list of grippers
		List<Part> allParts = solutionEditor.GetSolution().parts;
		Dictionary<Part, PartSimState> simStates = simulationDict;
		HeldGrippers = new();
		foreach(var part in allParts)
			foreach(var gripper in part.subparts)
				if(simStates[gripper].heldMolecule.HasValue())
					HeldGrippers.Add(gripper);
        // run the cycle
        //RunCycleGlyphsMain(isCycleStart);
        orig_RunCycleGlyphs(isCycleStart);

        // and then process things that happen after
		foreach(var action in QApi.ToRunAfterCycle)
			action((Sim)(object)this, isCycleStart);
	}
}