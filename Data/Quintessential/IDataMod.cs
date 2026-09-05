using Quintessential.Serialization;
using System.Collections.Generic;
using System.IO;

namespace Quintessential;

public interface IDataMod {
    
    public void LoadTags() {
        if (this is QuintessentialMod mod) {
            LoadTagsFromFile(mod);
        }
    }

    public static sealed void LoadTagsFromFile(QuintessentialMod mod) {
        if (File.Exists(Path.Combine(mod.Meta.PathToDirectory, "Content", "tags", "atomtags.jsonc"))) {
            DataSerializer.Deserialize<Dictionary<Identifier, AtomTag>>(Path.Combine(mod.Meta.PathToDirectory, "Content", "tags", "atomtags.jsonc"));
            Logger.Log($"Loaded atomtags.jsonc from '{mod.ModId}'");
        }
    }
}
