using MonoMod;
using Quintessential;
using SDL2;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

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
            Logger.LogNoTime("");
            Logger.LogNoTime(" -<>- Runtime information: ");
            Logger.LogNoTime(" Working directory   : " + Directory.GetCurrentDirectory());
            Logger.LogNoTime(" Process architecture: " + RuntimeInformation.ProcessArchitecture);
            Logger.LogNoTime(" Os      architecture: " + RuntimeInformation.OSArchitecture);
			Logger.LogNoTime(" Runtime identifier  : " + RuntimeInformation.RuntimeIdentifier);
            Logger.LogNoTime(" Runtime directory   : " + RuntimeEnvironment.GetRuntimeDirectory());
            Logger.LogNoTime(" Runtime version     : " + RuntimeEnvironment.GetSystemVersion());
            Logger.LogNoTime(" OS      version     : " + Environment.OSVersion);
            Logger.LogNoTime("");
            Logger.LogNoTime(" -<>- Exception:");
            Logger.LogNoTime(e.ToString());
            Logger.LogNoTime("");
            Logger.LogNoTime(" -<>- SDL Error:");
            Logger.LogNoTime(SDL.SDL_GetError());
        };
	}
}