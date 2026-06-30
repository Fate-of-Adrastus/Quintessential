using MonoMod;

[MonoModPatch("class_110")]
class patch_Settings {

	// settings init
	// disabling steam

	[PatchSettingsStaticInit]
	public static extern void orig_cctor();

	[MonoModConstructor]
	public static void cctor() {
		orig_cctor();
	}

    [RemoveReadOnly]
    public static readonly bool field_1010; // isDevEnv

    [RemoveReadOnly]
    public static readonly bool field_1014; // showLogWindow

    [RemoveReadOnly]
    public static readonly bool field_1022; // allowPseudo

    [RemoveReadOnly]
    public static readonly bool field_1026; // logSimulationState
}