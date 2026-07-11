using MonoMod;

class patch_AtomType
{

    // String atom type ID
    public string QuintAtomType;

    [MonoModReplace]
    public override bool Equals(object obj)
    {
        return obj is patch_AtomType type && this == type;
    }

    [MonoModReplace]
    public override int GetHashCode()
    {
        return ((AtomType)(object)this).byteId.GetHashCode() ^ QuintAtomType.GetHashCode();
    }

    [MonoModReplace]
    public static bool operator ==(patch_AtomType atomType1, patch_AtomType atomType2)
    {
        return (((AtomType)(object)atomType1).byteId == ((AtomType)(object)atomType2).byteId) && string.Equals(atomType1.QuintAtomType, atomType2.QuintAtomType);
    }

    [MonoModReplace]
    public static bool operator !=(patch_AtomType atomType1, patch_AtomType atomType2)
    {
        return !(atomType1 == atomType2);
    }
}