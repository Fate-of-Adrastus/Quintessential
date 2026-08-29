using System;
#pragma warning disable CS9113 // Parameter is unread

namespace MonoMod;

[MonoMod__SafeToCopy__]
[AttributeUsage(AttributeTargets.Method)]
public class MonoModWrapOperation(string TargetMethodName, string targetPoint, string TargetMetadata) : Attribute {

}
