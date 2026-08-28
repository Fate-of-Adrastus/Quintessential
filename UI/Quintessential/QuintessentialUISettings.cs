using Quintessential.Internal;
using Quintessential.Settings;
using YamlDotNet.Serialization;

namespace Quintessential;

public class QuintessentialUISettings {

    public static QuintessentialUISettings Instance => QuintessentialUI.Instance.Settings as QuintessentialUISettings;

    //[SettingsLabel("Take Screenshot")]
    //public Keybinding Screenshot = new("F12");

    [SettingsLabel("Dump Puzzles")]
    [YamlIgnore]
    public SettingsButton DumpPuzzles = Dumping.DumpVanillaPuzzles;

    [SettingsLabel("Dump Atom Sprites")]
    [YamlIgnore]
    public SettingsButton DumpAtomSprites = Dumping.DumpAtomSprites;
}
