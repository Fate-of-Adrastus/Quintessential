using Mono.Cecil;
using MonoMod.InlineRT;
using System;

namespace MonoMod;

[MonoModCustomAttribute(nameof(MonoModRules.RemoveReadOnly))]
class RemoveReadOnly : Attribute { }

static class MonoModRules {

    static MonoModRules() {
        MonoModRule.Modder.Log("Patching OM");
    }

    public static void RemoveReadOnly(FieldDefinition field, CustomAttribute attrib) {
        field.IsInitOnly = false;
    }
}