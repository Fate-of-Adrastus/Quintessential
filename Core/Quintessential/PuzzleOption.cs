using System.Collections.Generic;

namespace Quintessential;

public class PuzzleOption{

    // Puzzle options are always saved as LocStrings
    // booleans -> ID present or not
    // multi-choice -> {ID}__{choice}
    // atom -> {ID}__{atom ID}
    // part -> {ID}__{part ID}

    public Identifier ID;
    public LocString Name, SectionName;
	public int length;
    public PuzzleOptionType Type;

	private List<LocString> choices;

	public static PuzzleOption BoolOption(Identifier id, LocString name, LocString sectionName, int length){
		return new PuzzleOption{
			ID = id,
			Name = name,
			SectionName = sectionName,
			Type = PuzzleOptionType.Boolean,
            length = length,
        };
	}
	
	public static PuzzleOption MultiChoiceOption(Identifier id, LocString name, LocString sectionName, params LocString[] choices){
		return new PuzzleOption{
			ID = id,
			Name = name,
			SectionName = sectionName,
			Type = PuzzleOptionType.MultiChoice,
			choices = [.. choices]
        };
	}
	
	public static PuzzleOption PartTypeOption(Identifier id, LocString name, LocString sectionName){
		return new PuzzleOption{
			ID = id,
			Name = name,
			SectionName = sectionName,
			Type = PuzzleOptionType.Part
		};
	}
	
	public static PuzzleOption AtomTypeOption(Identifier id, LocString name, LocString sectionName){
		return new PuzzleOption{
			ID = id,
			Name = name,
			SectionName = sectionName,
			Type = PuzzleOptionType.Atom
		};
	}

	// Getters that each correspond to a PuzzleOptionType
	
	public bool EnabledIn(Puzzle from){
		return ((patch_Puzzle)(object)from).CustomPermissions?.Contains(ID) ?? false;
	}

	public string ChoiceIn(Puzzle from){
		foreach(string permission in ((patch_Puzzle)(object)from).CustomPermissions)
			if(permission.StartsWith(ID + "__"))
				return permission[(ID.ToString().Length + 2)..];
		return null;
	}

	public PartType PartIn(Puzzle from){
		string choice = ChoiceIn(from);
		foreach(PartType type in PartTypes.partTypes)
			if(type.id.Equals(choice))
				return type;

		return null;
	}

	public AtomType AtomIn(Puzzle from){
		string choice = ChoiceIn(from);
		foreach(AtomType type in AtomTypes.atoms)
			if(((patch_AtomType)(object)type).QuintAtomType.Equals(choice))
				return type;

		return null;
	}

	public void SetEnabledIn(Puzzle from, bool enabled){
		if(enabled)
			((patch_Puzzle)(object)from).CustomPermissions.Add(ID);
		else
			((patch_Puzzle)(object)from).CustomPermissions.Remove(ID);
	}
	
	public void SetChoiceIn(Puzzle from, string choice){
		var perms = ((patch_Puzzle)(object)from).CustomPermissions;
		perms.RemoveWhere(s => s.ToString().StartsWith(ID + "__"));
		perms.Add(ID + "__" + choice);
	}

	public void SetAtomIn(Puzzle from, AtomType atom){
		SetChoiceIn(from, ((patch_AtomType)(object)atom).QuintAtomType);
	}

	public void SetPartIn(Puzzle from, PartType part){
		SetChoiceIn(from, part.id);
	}
}

public enum PuzzleOptionType{
	Boolean,
	MultiChoice,
	Part,
	Atom,
}