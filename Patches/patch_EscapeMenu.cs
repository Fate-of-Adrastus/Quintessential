using MonoMod;
using Quintessential;

#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

[MonoModPatch("EscapeScreen")]
class patch_PauseScreen {

	public extern void orig_RenderFrame(float deltaTime);

	public void RenderFrame(float deltaTime) {
		if(GameLogic.instance.GetLastScreen() is ModsScreen)
			return;
        orig_RenderFrame(deltaTime);
		float num = 65f;
		Vector2 vector2_1 = new(570f, 440f);
		Vector2 vector2_2 = (InputManager.screenSize / 2 - vector2_1 / 2).Rounded();
		Vector2 vector2_3 = new(161f, 256f);
		Vector2 vector2_4 = vector2_3 + new Vector2(0.0f, -num);
		Vector2 vector2_5 = vector2_4 + new Vector2(0.0f, -num);
		Vector2 vector2_6 = vector2_5 + new Vector2(0.0f, -num * 2);
		if(UIUtils.TextButton(Translations.Translate("Mods"), vector2_2 + vector2_6).RenderAndCheckIfPressed(true,true)) {
			// show mod options
			UI.OpenScreen(new ModsScreen());
		}
	}
}

