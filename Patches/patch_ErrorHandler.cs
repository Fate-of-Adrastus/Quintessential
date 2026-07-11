using MonoMod;
using Quintessential;
using System;

#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

[MonoModPatch("ErrorHandler")]
class patch_ErrorHandler {

	// error logging
	// replaces the regular method (opening a (broken by string parsing?) website) with logging
	
	[MonoModReplace]
	public static void BindHandler() {
		AppDomain.CurrentDomain.UnhandledException += (sender, args) => {
			Logger.Log("Encountered an error!");
			Exception e = args.ExceptionObject as Exception;
			Logger.Log(e.ToString());
		};
	}
}