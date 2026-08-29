using System.Collections.Generic;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace Quintessential.Serialization;

public class CampaignModel {

	public string Name { get; set; }
	public string TitleKey { get; set; }
	public IList<ChapterModel> Chapters { get; set; }

	[YamlIgnore] [JsonIgnore]
	public string Path = "";
}

public class ChapterModel {

	public string TitleKey { get; set; }
    public string SubtitleKey { get; set; }
	public string PlaceKey { get; set; }
	public string Background { get; set; }
	public IList<EntryModel> Entries { get; set; }

    // TODO: wheel icons
}

public class EntryModel {

	// TODO: multiple requirements, documents, tutorials

	public string Type { get; set; } = "puzzle";
	public string ID { get; set; }
	public string TitleKey { get; set; }
    public string Puzzle { get; set; }
	public string Requires { get; set; }
	public string Icon { get; set; }
	public string IconSmall { get; set; }
	public bool NoStoryPanel{ get; set; }
}
