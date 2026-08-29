using System.Collections.Generic;

using YamlDotNet.Serialization;

namespace Quintessential.Serialization;

public class JournalModel {

	public string TitleKey { get; set; }

    public string PuzzleBackgroundLarge { get; set; }
	public string PuzzleBackgroundSmall { get; set; }

	public List<JournalChapterModel> Chapters = [];

	[YamlIgnore]
	public string Path = "";

	[YamlIgnore]
	public Texture PuzzleBackgroundSmallTex, PuzzleBackgroundLargeTex;
}

public class JournalChapterModel {

	public string TitleKey { get; set; }

    public string DescriptionKey { get; set; }

    public List<string> Puzzles = [];
}
