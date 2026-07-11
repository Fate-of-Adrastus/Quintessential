#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE1006 // Naming Styles

using MonoMod;
using Quintessential;

[MonoModPatch("SolutionRecorderScreen")]
class patch_SolutionRecorderScreen {

	[PatchGifRecorderFrame]
	[MonoModIgnore]
	public extern void RenderFrame(float detaTime);

	// name is used in MonoModRules
	private static void MarkOnFrame(){
		var markerPos = new Vector2(826 - 60 - 40, 647 - 61);
		var verPos = new Vector2(826 - 60 - 40 - 20, 647 - 40);
        TextureRenderer.Render(Assets.textures.field_81.field_613.field_632, markerPos);
        TextureRenderer.RenderText(QuintessentialLoader.VersionString, verPos, Assets.fonts.crimson_16_5, Color.LightGray, (TextAlignment)1, 1f, 0.6f, float.MaxValue, float.MaxValue, 0, new Color(), Assets.textures.white, int.MaxValue, true, true);
	}
}