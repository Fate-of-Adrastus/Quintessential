using MonoMod;
using Quintessential;

class patch_AtomType : AtomType
{

    // String atom type ID
    public Identifier QuintAtomType;

    [MonoModReplace]
    public override bool Equals(object obj)
    {
        return obj is patch_AtomType type && this == type;
    }

    [MonoModReplace]
    public override int GetHashCode()
    {
        return byteId.GetHashCode() ^ QuintAtomType.GetHashCode();
    }

    [MonoModReplace]
    public static bool operator ==(patch_AtomType atomType1, patch_AtomType atomType2)
    {
        return (atomType1.byteId == atomType2.byteId) && atomType1.QuintAtomType == atomType2.QuintAtomType;
    }

    [MonoModReplace]
    public static bool operator !=(patch_AtomType atomType1, patch_AtomType atomType2)
    {
        return !(atomType1 == atomType2);
    }
}