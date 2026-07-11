using MonoMod;
using Quintessential;

#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it



public class patch_JournalScreen{

	public static int currentJournal;
	
	private static Texture JournalGoLeft, JournalGoLeftHover, JournalGoRight, JournalGoRightHover;
	
	// mirror real version
	private static int volumeCount;
	
	[PatchJournalScreen]
	public extern void orig_RenderFrame(float deltaTime);
	public void RenderFrame(float deltaTime){
        orig_RenderFrame(deltaTime);

		if(QuintessentialLoader.AllJournals.Count == 1)
			return;
		
		JournalGoLeft ??= AssetLoaderHelper.LoadTexture("Quintessential/journal_go_left");
		JournalGoLeftHover ??= AssetLoaderHelper.LoadTexture("Quintessential/journal_go_left_hover");
		JournalGoRight ??= AssetLoaderHelper.LoadTexture("Quintessential/journal_go_right");
		JournalGoRightHover ??= AssetLoaderHelper.LoadTexture("Quintessential/journal_go_right_hover");
		
		Vector2 size = new Vector2(1516f, 922f);
		Vector2 corner = (Input.ScreenSize() / 2 - size / 2 + new Vector2(-2f, -11f)).Rounded();
		Vector2 lPos = corner + new Vector2(84, 812f);
		Vector2 rPos = corner + new Vector2(188, 812f);
		bool inLeftBound = Bounds2.WithSize(lPos, JournalGoLeft.size.ToVector2()).Contains(Input.MousePos());
		bool inRightBound = Bounds2.WithSize(rPos, JournalGoRight.size.ToVector2()).Contains(Input.MousePos());
		TextureRenderer.Render(inLeftBound ? JournalGoLeftHover : JournalGoLeft, lPos);
        TextureRenderer.Render(inRightBound ? JournalGoRightHover : JournalGoRight, rPos);
        UI.DrawText($"{currentJournal + 1}/{QuintessentialLoader.AllJournals.Count}", corner + new Vector2(157, 824f), UI.Text, UI.TextColor, (TextAlignment)1);
        //UI.DrawTexture(inLeftBound ? JournalGoLeftHover : JournalGoLeft, lPos);
        //UI.DrawTexture(inRightBound ? JournalGoRightHover : JournalGoRight, rPos);
        //UI.DrawText($"{currentJournal + 1}/{QuintessentialLoader.AllJournals.Count}", corner + new Vector2(157, 824f), UI.Text, UI.TextColor, TextAlignment.Centred);

		if(Input.IsLeftClickPressed() && (inLeftBound || inRightBound)){
            Assets.sounds.field_1821.method_28(1f);
			
			if(inLeftBound){
				var next = currentJournal - 1;
				if(next < 0)
					next += QuintessentialLoader.AllJournals.Count;
				currentJournal = next;
			}

			if(inRightBound){
				var next = currentJournal + 1;
				if(next >= QuintessentialLoader.AllJournals.Count)
					next = 0;
				currentJournal = next;
			}

			JournalVolumes.volumes = QuintessentialLoader.AllJournals[currentJournal].ToArray();
            volumeCount = JournalVolumes.volumes.Length - 1;
			UI.InstantCloseScreen();
			UI.OpenScreen(new JournalScreen(false));
		}
	}

	[MonoModIgnore]
	[PatchJournalPuzzleBackgrounds]
	private extern void RenderPuzzleSelect(Puzzle puzzle, Vector2 pos, bool isLarge);

	public static void ResetPosition(){
		currentJournal = 0;
        volumeCount = JournalVolumes.volumes.Length - 1;
	}

	// found by name in MonoModRules
	public static string CurrentJournalName(){
		return currentJournal == 0 ? "The Journal of Alchemical Engineering" : QuintessentialLoader.ModJournalModels[currentJournal - 1].Title;
	}

	public static Texture CurrentJournalBg(Texture before, bool large){
		if(currentJournal == 0)
			return before;
		var journal = QuintessentialLoader.ModJournalModels[currentJournal - 1];
		return large switch{
			true when !string.IsNullOrWhiteSpace(journal.PuzzleBackgroundLarge) => (journal.PuzzleBackgroundLargeTex ??= AssetLoaderHelper.LoadTexture(journal.PuzzleBackgroundLarge)),
			false when !string.IsNullOrWhiteSpace(journal.PuzzleBackgroundSmall) => (journal.PuzzleBackgroundSmallTex ??= AssetLoaderHelper.LoadTexture(journal.PuzzleBackgroundSmall)),
			_ => before
		};
	}
}