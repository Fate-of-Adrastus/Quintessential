using Mono.Cecil;
using MonoMod.InlineRT;
using System;

namespace MonoMod;

[MonoModCustomAttribute(nameof(MonoModRules.RemoveReadOnly))]
[AttributeUsage(AttributeTargets.Field)]
class MonoModRemoveReadOnly : Attribute { }

[MonoModCustomAttribute(nameof(MonoModRules.Internal))]
[AttributeUsage(AttributeTargets.Field)]
class MonoModInternal : Attribute { }

static class MonoModRules {

    static MonoModRules() {
        MonoModRule.Modder.Log("Patching OM");
    }

    public static void RemoveReadOnly(FieldDefinition field, CustomAttribute attrib) {
        field.IsInitOnly = false;
    }
    public static void Internal(FieldDefinition field, CustomAttribute attrib) {
        field.IsAssembly = true;
    }
}