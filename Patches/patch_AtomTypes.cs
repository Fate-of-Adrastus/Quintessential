using MonoMod;

#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

[MonoModPatch("AtomTypes")]
class patch_AtomTypes{
	
	public static extern void orig_Init();

	public static void Init(){
        orig_Init();
		((patch_AtomType)(object)AtomTypes.salt).QuintAtomType = "salt";
		((patch_AtomType)(object)AtomTypes.air).QuintAtomType = "air";
		((patch_AtomType)(object)AtomTypes.earth).QuintAtomType = "earth";
		((patch_AtomType)(object)AtomTypes.fire).QuintAtomType = "fire";
		((patch_AtomType)(object)AtomTypes.water).QuintAtomType = "water";
		((patch_AtomType)(object)AtomTypes.quicksilver).QuintAtomType = "quicksilver";
		((patch_AtomType)(object)AtomTypes.lead).QuintAtomType = "lead";
		((patch_AtomType)(object)AtomTypes.copper).QuintAtomType = "copper";
		((patch_AtomType)(object)AtomTypes.tin).QuintAtomType = "tin";
		((patch_AtomType)(object)AtomTypes.iron).QuintAtomType = "iron";
		((patch_AtomType)(object)AtomTypes.silver).QuintAtomType = "silver";
		((patch_AtomType)(object)AtomTypes.gold).QuintAtomType = "gold";
		((patch_AtomType)(object)AtomTypes.vitae).QuintAtomType = "vitae";
		((patch_AtomType)(object)AtomTypes.mors).QuintAtomType = "mors";
		((patch_AtomType)(object)AtomTypes.repeat).QuintAtomType = "repeat";
		((patch_AtomType)(object)AtomTypes.quintessence).QuintAtomType = "quintessence";
	}
}