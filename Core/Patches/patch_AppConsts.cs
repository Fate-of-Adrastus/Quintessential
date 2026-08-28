#pragma warning disable CS0649 // never assigned to

using MonoMod;

class patch_AppConsts {

	// settings init
	// disabling steam

	//[PatchSettingsStaticInit]
	//public static extern void orig_cctor();

	//[MonoModConstructor]
	//public static void cctor() {
	//	orig_cctor();
	//}

    [RemoveReadOnly]
    public static readonly bool isDevEnv; // isDevEnv

    [RemoveReadOnly]
    public static readonly bool showLogWindow; // showLogWindow

    [RemoveReadOnly]
    public static readonly bool allowPseudo; // allowPseudo

    [RemoveReadOnly]
    public static readonly bool logSimulationState; // logSimulationState
}