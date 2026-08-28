using System.Collections.Generic;
using MonoMod.Utils;

namespace Quintessential;

public static class UI {

	#region Constants

	public static readonly LocalizedFont Title = Assets.fonts.crimson_21;
	public static readonly LocalizedFont Text = Assets.fonts.crimson_16_5;
	public static readonly LocalizedFont SubTitle = Assets.fonts.crimson_13;

	public static readonly Color TextColor = class_181.field_1718;

	public static readonly Texture BackgroundLarger = AssetLoaderHelper.LoadTexture("textures/background_larger");

	#endregion

	#region Texture drawing methods

	public static void DrawTexture(Texture texture, Vector2 pos) {
		TextureRenderer.Render(texture, pos);
	}

	public static void DrawRepeatingTexture(Texture texture, Vector2 pos, Vector2 size) {
		TextureRenderer.RenderMasked(texture, Color.White, pos, Bounds2.WithSize(pos, size));
	}

	public static void DrawResizableTexture(Texture texture, Vector2 pos, Vector2 size) {
		TextureRenderer.Render9Slice(texture, Color.White, pos, size);
	}

	#endregion

	#region Text drawing methods

	public static Bounds2 DrawText(string text, Vector2 pos, StyleableFont font, Color color, TextAlignment alignment, float maxWidth = float.MaxValue, float ellipsesCutoff = float.MaxValue) {
		return TextureRenderer.RenderText(text, pos, font, color, alignment, 1f, 0.6f, maxWidth, ellipsesCutoff, 0, new Color(), null, int.MaxValue, true, true);
	}

	public static void DrawHeader(string text, Vector2 pos, int width, bool a, bool b) {
		UIUtils.RenderScreenTitle(Translations.Translate(text), pos, width, true, true);
	}

	#endregion

	#region Button drawing methods

	public static bool DrawAndCheckCloseButton(Vector2 framePos, Vector2 frameSize, Vector2 closeButtonOffset) {
		return UIUtils.CloseButton(framePos, frameSize, frameSize - closeButtonOffset);
	}

	public static bool DrawAndCheckSolutionButton(string text, string subtext, Vector2 pos, int width, bool selected) {
		return UIUtils.SolutionButton(text, subtext == null ? MaybeHelper.empty : Maybe<string>.From(subtext), pos, width, selected).RenderAndCheckIfPressed(true, true);
	}

	public static bool DrawAndCheckBoxButton(string text, Vector2 pos) {
		return UIUtils.TextButton(text, pos).RenderAndCheckIfPressed(true, true);
	}

	public static bool DrawAndCheckSimpleButton(string text, Vector2 pos, Vector2 size) {
		return UIUtils.class_149.method_348(text, pos, size).RenderAndCheckIfPressed(true, true);
	}

	#endregion

	#region Screen stack

	public static void HandleCloseButton() {
		CloseScreen();
		// Play close sound
		Assets.sounds.ui_modal_close.method_28(1f);
	}

	public static void CloseScreen() {
		GameLogic.instance.fadeBackground = false;
		GameLogic.instance.PopScreen();
	}

	public static void InstantCloseScreen() {
		GameLogic.instance.PopScreens(1);
	}

	public static void OpenScreen(IScreen toOpen) {
		GameLogic.instance.PushScreen(toOpen, MaybeHelper.empty, MaybeHelper.empty);
	}

	#endregion

	#region UI helpers

	public static void DrawUiBackground(Vector2 pos, Vector2 size) {
		DrawRepeatingTexture(Assets.textures.window.background, pos, size);
	}
	
	public static void DrawLargeUiBackground(Vector2 pos, Vector2 size) {
		DrawRepeatingTexture(BackgroundLarger, pos, size);
	}

	public static void DrawUiFrame(Vector2 pos, Vector2 size) {
		DrawResizableTexture(Assets.textures.window.frame, pos, size);
	}

	public static bool DrawCheckbox(Vector2 pos, string label, bool enabled) {
		Bounds2 boxBounds = Bounds2.WithSize(pos, new Vector2(36f, 37f));
		Bounds2 labelBounds = DrawText(label, pos + new Vector2(45f, 13f), SubTitle, TextColor, (TextAlignment)0);
		if(enabled)
			DrawTexture(Assets.textures.UI.checkbox_fill, boxBounds.Min);
		if(boxBounds.Contains(InputManager.MousePos()) || labelBounds.Contains(InputManager.MousePos())) {
			DrawTexture(Assets.textures.UI.checkbox_hover, boxBounds.Min);
			if(!InputManager.IsClickPressed(MouseButtonType.LeftClick))
				return false;
            Assets.sounds.click_button.method_28(1f);
			return true;
		}
		DrawTexture(Assets.textures.UI.checkbox, boxBounds.Min);
		return false;
	}
	
	#endregion

	#region Texture control methods

	public static Texture AssignOffset(Texture tex, Vector2 offset){
		new DynamicData(typeof(TextureOffsets)).Get<Dictionary<Texture, Vector2>>("offsets")[tex] = offset;
		return tex;
	}
	
	public static Texture AssignCentre(Texture tex, Vector2 offset){
		new DynamicData(typeof(TextureOffsets)).Get<Dictionary<Texture, Vector2>>("scales")[tex] = offset;
		return tex;
	}

	#endregion
}
