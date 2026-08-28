using System.Collections.Generic;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace Quintessential.Serialization;

public class CampaignModel {

	public string Name { get; set; } // TODO: add localisation compat
	public string Title { get; set; }
	public IList<ChapterModel> Chapters { get; set; }

	[YamlIgnore] [JsonIgnore]
	public string Path = "";
}

public class ChapterModel {

	public string Title { get; set; } // TODO: add localisation compat
    public string Subtitle { get; set; }
	public string Place { get; set; }
	public string Background { get; set; }
	public IList<EntryModel> Entries { get; set; }

    // TODO: wheel icons
}

public class EntryModel {

	// TODO: multiple requirements, documents, tutorials

	public string Type { get; set; } = "puzzle";
	public string ID { get; set; }
	public string Title { get; set; } // TODO: add localisation compat
    public string Puzzle { get; set; }
	public string Requires { get; set; }
	public string Icon { get; set; }
	public string IconSmall { get; set; }
	public bool NoStoryPanel{ get; set; }
}
