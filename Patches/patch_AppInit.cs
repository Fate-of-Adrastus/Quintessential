using MonoMod;
using Quintessential;
using System;
using System.Linq;
using System.Reflection;
using AppInit = class_119;

#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

[MonoModPatch("class_119")]
class patch_AppInit {
    public static extern void orig_method_220(string[] param_3295);

    public static void method_220(string[] param_3295) {

        Type appConsts = typeof(class_110);
        if (appConsts != null) {
            if (param_3295.Contains("-devEnv")) {
                appConsts.GetField("field_1010").SetValue(null, true);
                appConsts.GetField("field_1026").SetValue(null, true); // LogSimState
            }
            if (param_3295.Contains("-openGameLogWindow")) {
                appConsts.GetField("field_1014").SetValue(null, true);
            }
            if (param_3295.Contains("-pseudoLang")) {
                appConsts.GetField("field_1022").SetValue(null, true);
            }
        }

        orig_method_220(param_3295);
    }
}
