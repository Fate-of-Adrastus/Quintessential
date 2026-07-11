using Quintessential;
using System;
using System.IO;

#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

class patch_Renderer {

	// checks mods for textures before vanilla

	public static extern bool orig_GetImageByExtension(Texture texture);

	public static bool GetImageByExtension(Texture texture) {
		string origPath = null;
		if(texture.sourceFile.HasValue() /*Exists*/){
			origPath = texture.sourceFile.GetValue();
			if(texture.sourceFile.GetValue() /*Get*/.StartsWith("Content")) {
				for(int i = QuintessentialLoader.ModContentDirectories.Count - 1; i >= 0; i--){
					string dir = QuintessentialLoader.ModContentDirectories[i];
					try{
                        texture.sourceFile = Path.Combine(dir, origPath);
						return orig_GetImageByExtension(texture);
					}catch(Exception e){
						HandleException(e);
					}finally {
                        texture.sourceFile = origPath;
					}
				}
			}
		}
		try{
			return orig_GetImageByExtension(texture);
		}catch(Exception e) {
			HandleException(e);
		}
		// none match -> use missing texture
		try{
			Logger.Log($"Texture {origPath} does not exist, using fallback texture");
            texture.sourceFile = Path.Combine(QuintessentialLoader.ModContentDirectories[0], "Content", "Quintessential", "missing");
			return orig_GetImageByExtension(texture);
		}finally{
            texture.sourceFile = origPath;
		}
	}

	private static void HandleException(Exception e){
		if(e.Message.StartsWith("Texture file not found!"))
			return;
		if(e is OpusMagnumException && e.Message.StartsWith("Failed to load PNG file:"))
			throw new Exception($"SDL failed to load a PNG: \"{SDL2.SDL.SDL_GetError()}\"", e);
		throw e;
	}
}