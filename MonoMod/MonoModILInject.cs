using System;
#pragma warning disable CS9113 // Parameter is unread

namespace MonoMod;

[MonoMod__SafeToCopy__]
[AttributeUsage(AttributeTargets.Method)]
public class MonoModILInject(string InjectedModifier) : Attribute {

}
