using SDL2;

namespace Quintessential;

/// <summary>
/// Generic info popup screen.
/// </summary>
public class NoticeScreen : IScreen {

	private readonly string Title, Tooltip;

	public NoticeScreen(string title, string tooltip) {
		Title = title;
		Tooltip = tooltip;
	}

	public bool PreventLowerScreenUpdates() {
		return false;
	}

	public void OnOpenOrClose(bool isOpening) {
		// Add gray BG
		GameLogic.instance.fadeBackGround = true;
	}

	public void Reset() {
		
	}

	public void RenderFrame(float deltaTime) {
		UI.DrawText(Title, (Input.ScreenSize() / 2) + new Vector2(0, 120), UI.Title, Color.White, (TextAlignment)1);
		UI.DrawText(Tooltip, Input.ScreenSize() / 2, UI.SubTitle, class_181.field_1718, (TextAlignment)1);
		if(Input.IsSdlKeyPressed(SDL.SDLKey.SDLK_ESCAPE) || UI.DrawAndCheckBoxButton("OK", (Input.ScreenSize() / 2) + new Vector2(-130, -160)))
			UI.CloseScreen();
	}
}