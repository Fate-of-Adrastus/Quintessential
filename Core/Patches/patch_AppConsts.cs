#pragma warning disable CS0649 // never assigned to

using MonoMod;

class patch_AppConsts {

    [RemoveReadOnly]
    public static readonly bool isDevEnv; // isDevEnv

    [RemoveReadOnly]
    public static readonly bool showLogWindow; // showLogWindow

    [RemoveReadOnly]
    public static readonly bool allowPseudo; // allowPseudo

    [RemoveReadOnly]
    public static readonly bool logSimulationState; // logSimulationState
}