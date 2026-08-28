#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

class patch_AtomTypes{
	
	public static extern void orig_Init();

	public static void Init(){
        orig_Init();
		((patch_AtomType)(object)AtomTypes.salt).QuintAtomType = "om:salt";
		((patch_AtomType)(object)AtomTypes.air).QuintAtomType = "om:air";
		((patch_AtomType)(object)AtomTypes.earth).QuintAtomType = "om:earth";
		((patch_AtomType)(object)AtomTypes.fire).QuintAtomType = "om:fire";
		((patch_AtomType)(object)AtomTypes.water).QuintAtomType = "om:water";
		((patch_AtomType)(object)AtomTypes.quicksilver).QuintAtomType = "om:quicksilver";
		((patch_AtomType)(object)AtomTypes.lead).QuintAtomType = "om:lead";
		((patch_AtomType)(object)AtomTypes.copper).QuintAtomType = "om:copper";
		((patch_AtomType)(object)AtomTypes.tin).QuintAtomType = "om:tin";
		((patch_AtomType)(object)AtomTypes.iron).QuintAtomType = "om:iron";
		((patch_AtomType)(object)AtomTypes.silver).QuintAtomType = "om:silver";
		((patch_AtomType)(object)AtomTypes.gold).QuintAtomType = "om:gold";
		((patch_AtomType)(object)AtomTypes.vitae).QuintAtomType = "om:vitae";
		((patch_AtomType)(object)AtomTypes.mors).QuintAtomType = "om:mors";
		((patch_AtomType)(object)AtomTypes.repeat).QuintAtomType = "om:repeat";
		((patch_AtomType)(object)AtomTypes.quintessence).QuintAtomType = "om:quintessence";
	}
}