#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

using Quintessential;
using Quintessential.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class patch_Translations {
    private static Dictionary<string, LocString> translationDict;

    public static extern void orig_Init();
    public static void Init() {
        orig_Init();

        // Loading localisation data into LocalisationLayer.GlobalLayer
        foreach (var dir in QuintessentialLoader.ModContentDirectories) {
            var langDirPath = Path.Combine(dir, "Content", "lang");
            if (Directory.Exists(langDirPath)) {
                foreach (var file in Directory.GetFiles(langDirPath)) {
                    var Language = Translations.countryCodes.FirstOrDefault(
                        code => Path.GetFileName(file) == code.Value + ".jsonc",
                        new(0, null));

                    if (Language.Value != null) {
                        LocalisationLayer.CurrentFileLanguage = Language.Key;
                        DataSerializer.Deserialize<LocalisationLayer>(file);
                    }
                }
            }
        }
        LocalisationLayer.GlobalLayer.AddSelfAndSubToDictionary(translationDict);
    }
}
