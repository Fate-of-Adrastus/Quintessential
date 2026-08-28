#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

public class patch_Molecule{

	// Copy molecule names when copying the molecule
	private extern Molecule orig_Clone();
	public Molecule Clone(){
		Molecule ret = orig_Clone();
		ret.displayName = ((Molecule)(object)this).displayName;
		return ret;
	}
}