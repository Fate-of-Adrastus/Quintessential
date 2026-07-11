using SDL2;

namespace Quintessential;

/// <summary>
/// Helper class containing functions for querying keyboard or mouse input.
/// </summary>
public static class Input {

	#region Keyboard input

	public static bool IsShiftHeld() {
		return InputManager.IsModifierKeyHeld(0);
	}

	public static bool IsControlHeld() {
		return InputManager.IsModifierKeyHeld((ModifierKeyType)1);
	}

	public static bool IsAltHeld() {
		return InputManager.IsModifierKeyHeld((ModifierKeyType)2);
	}

	public static bool IsSdlKeyPressed(SDL.SDLKey key) {
		return InputManager.IsKeyPressed(key);
	}

	public static bool IsSdlKeyReleased(SDL.SDLKey key) {
		return InputManager.IsKeyReleased(key);
	}

	public static bool IsSdlKeyHeld(SDL.SDLKey key) {
		return InputManager.IsKeyHeld(key);
	}

	public static SDL.SDLKey GetSdlKeyForCharacter(string character) {
		return SDL.SDL_GetKeyFromName(character);
	}

	public static bool IsKeyPressed(string key) {
		return InputManager.IsKeyPressed(GetSdlKeyForCharacter(key));
	}

	public static bool IsKeyReleased(string key) {
		return InputManager.IsKeyReleased(GetSdlKeyForCharacter(key));
	}

	public static bool IsKeyHeld(string key) {
		return InputManager.IsKeyHeld(GetSdlKeyForCharacter(key));
	}

	#endregion

	#region Mouse input

	public static Vector2 MousePos() {
		return InputManager.MousePos();
	}

	// Not sure if there is functionality for other values, like "(enum_142) 0" or  "(enum_142) 4"

	public static bool IsLeftClickHeld() {
		return InputManager.IsClickHeld((MouseButtonType)1);
	}
	public static bool IsLeftClickPressed() {
		return InputManager.IsClickPressed((MouseButtonType)1);
	}
	public static bool IsLeftClickReleased() {
		return InputManager.IsClickReleased((MouseButtonType)1);
	}

	public static bool IsMiddleClickHeld() {
		return InputManager.IsClickHeld((MouseButtonType)2);
	}
	public static bool IsMiddleClickPressed() {
		return InputManager.IsClickPressed((MouseButtonType)2);
	}
	public static bool IsMiddleClickReleased() {
		return InputManager.IsClickReleased((MouseButtonType)2);
	}

	public static bool IsRightClickHeld() {
		return InputManager.IsClickHeld((MouseButtonType)3);
	}
	public static bool IsRightClickPressed() {
		return InputManager.IsClickPressed((MouseButtonType)3);
	}
	public static bool IsRightClickReleased() {
		return InputManager.IsClickReleased((MouseButtonType)3);
	}

	#endregion

	#region Other

	public static Vector2 ScreenSize() {
		return InputManager.screenSize;
	}

	#endregion
}
