using System;
using System.Collections.Generic;

namespace Quintessential;

public class AtomTag : Tag {
    public static readonly string FileName = "atomTags.jsonc";
    public AtomTag(Identifier id, bool isTable = false) : base(id, isTable) { }


    public static Dictionary<Identifier, AtomTag> AtomTags = [];
    public static void DumpTags() {
        DumpTags(AtomTags, FileName);
    }


    public bool HasAtom(AtomType atom) => HasEntry(atom.QuintAtomType);
    public bool HasAtom(Atom atom) => HasEntry(atom.atomType.QuintAtomType);
    public bool HasAtom(AtomReference atom ) => HasEntry(atom.atomType.QuintAtomType);
    public bool HasAtom(AtomType atom, out Identifier? mapped) => HasEntry(atom.QuintAtomType, out mapped);
    public bool HasAtom(Atom atom, out Identifier? mapped) => HasEntry(atom.atomType.QuintAtomType, out mapped);
    public bool HasAtom(AtomReference atom, out Identifier? mapped) => HasEntry(atom.atomType.QuintAtomType, out mapped);
    public Identifier? GetMapped(AtomType atom) => GetMapped(atom.QuintAtomType);
    public Identifier? GetPair(Atom atom) => GetMapped(atom.atomType.QuintAtomType);
    public Identifier? GetMapped(AtomReference atom) => GetMapped(atom.atomType.QuintAtomType);

}

public class AtomTagJsonConverter : TagJsonConverter<AtomTag> {
    private static AtomTagJsonConverter Instance;
    private static bool wasInit = false;
    public static AtomTagJsonConverter Get() {
        if (!wasInit) {
            Instance = new(AtomTag.AtomTags, (id, isTable) => new AtomTag(id, isTable));
            wasInit = true;
        }
        return Instance;
    }

    private AtomTagJsonConverter(Dictionary<Identifier, AtomTag> GlobalTags, Func<Identifier, bool, AtomTag> CtorForType) : base(GlobalTags, CtorForType) {}
}
