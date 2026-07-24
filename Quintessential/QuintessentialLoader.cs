using Ionic.Zip;
using Quintessential.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace Quintessential;

public class QuintessentialLoader
{

    public static readonly string VersionString = "0.5.6";
    public static readonly int VersionNumber = 13;

    public static string PathLightning;
    public static string PathMods, PathUnpackedMods;
    public static string PathBlacklist;
    public static string PathModSaves;
    public static string PathScreenshots;

    public static List<QuintessentialMod> CodeMods = new();
    public static List<ModMeta> Mods = new();
    public static List<string> ModContentDirectories = new();
    public static List<string> ModPuzzleDirectories = new();

    public static List<Campaign> AllCampaigns = new();
    public static Campaign VanillaCampaign;
    public static List<List<JournalVolume>> AllJournals = new();
    public static List<JournalVolume> VanillaJournal;

    public static ModMeta QuintessentialModMeta;
    public static QuintessentialMod QuintessentialAsMod;

    public static List<CampaignModel> ModCampaignModels = new();
    public static List<JournalModel> ModJournalModels = new();

    private static List<string> blacklisted = new();
    private static List<ModMeta> loaded = new();
    private static List<ModMeta> waiting = new();

    private static readonly string zipExtractSuffix = "__quintessential_from_zip";
    private static readonly string quintAssetFolder = "__quintessential_assets";

    public static void PreInit()
    {
        try
        {
            PathLightning = Path.GetDirectoryName(typeof(GameLogic).Assembly.Location);
            PathMods = Path.Combine(PathLightning, "Mods");
            PathUnpackedMods = Path.Combine(PathLightning, "UnpackedMods");
            PathScreenshots = Path.Combine(PathLightning, "Screenshots");

            Logger.Init();
            Logger.Log($"Quintessential v{VersionString} ({VersionNumber})");
            Logger.Log("Starting pre-init loading.");

            QApi.Init();

            if (!Directory.Exists(PathMods))
                Directory.CreateDirectory(PathMods);

            if (Directory.Exists(PathUnpackedMods))
                Directory.Delete(PathUnpackedMods, true);
            Directory.CreateDirectory(PathUnpackedMods);

            PathBlacklist = Path.Combine(PathMods, "blacklist.txt");
            if (File.Exists(PathBlacklist))
                blacklisted = File.ReadAllLines(PathBlacklist).Select(l => (l.StartsWith("#") ? "" : l).Trim()).ToList();
            else
            {
                File.WriteAllText(PathBlacklist, @"# This is the blacklist. Lines starting with # are ignored.
ExampleFolderThatIWantToBlacklist
SomeZipIDontLike.zip");
            }

            // Find mods in Mods/
            // Delete legacy quintessential extracted zips and assets
            CleanupLegacyExtractedData();

            // Add Quintessential mod & mod meta
            QuintessentialModMeta = new ModMeta
            {
                Name = "Quintessential",
                Version = new Version(VersionString)
            };
            Mods.Add(QuintessentialModMeta);
            QuintessentialAsMod = new Internal.QuintessentialAsMod
            {
                Meta = QuintessentialModMeta,
                Settings = new QuintessentialSettings()
            };
            CodeMods.Add(QuintessentialAsMod);

            // Extract bundled assets
            Logger.Log("Extracting Quintessential resources...");
            string outDir = Path.Combine(PathUnpackedMods, quintAssetFolder, "Content", "Quintessential");
            Directory.CreateDirectory(outDir);
            ResourceManager manager = new("Properties.Resources", typeof(Renderer).Assembly);
            var set = manager.GetResourceSet(CultureInfo.InvariantCulture, true, true);
            foreach (object item in set)
            {
                if (item is DictionaryEntry de)
                {
                    string name = (string)de.Key;
                    using var toStream = File.OpenWrite(Path.Combine(outDir, name));
                    byte[] content = (byte[])de.Value;
                    toStream.Write(content, 0, content.Length);
                }
            }
            ModContentDirectories.Add(Path.Combine(PathUnpackedMods, quintAssetFolder));

            Logger.Log("Finding mods to load...");
            // Unzip zips
            string[] files = Directory.GetFiles(PathMods);
            foreach (var file in files)
            {
                string filename = Path.GetFileName(file);
                if (blacklisted.Contains(filename))
                    continue;
                if (filename.EndsWith(".zip"))
                    FindZipMod(file);
            }

            // Find folder mods
            string[] folders = Directory.GetDirectories(PathMods);
            foreach (var folder in folders)
            {
                string filename = Path.GetFileName(folder);
                if (blacklisted.Contains(filename))
                    continue;
                FindFolderMod(folder);
            }

            // Load mods
            Logger.Log("Loading mods...");

            Logger.Log("Stage 1: Searching for duplicates");
            HashSet<string> names = new();
            foreach (ModMeta mod in Mods)
            {
                if (names.Contains(mod.Name))
                {
                    throw new Exception("Duplicate mod named " + mod.Name + " found, use the blacklist to only permit at most one.");
                }
                names.Add(mod.Name);
            }

            bool Contains(ModMeta.Dependency dep, List<ModMeta> list)
                => list.Any(m => m.Name.Equals(dep.Name) && m.Version >= dep.Version);
            bool ContainsOutdated(ModMeta.Dependency dep, List<ModMeta> list)
                => list.Any(m => m.Name.Equals(dep.Name) && m.Version < dep.Version);

            Logger.Log("Stage 2: Removing outdated");
            List<ModMeta> ToRemove = new();
            do
            {
                ToRemove.Clear();
                foreach (ModMeta mod in Mods)
                {
                    bool missingDependencies = false;
                    ModMeta.Dependency example = null;
                    foreach (ModMeta.Dependency dep in mod.Dependencies)
                    {
                        if (!Contains(dep, Mods))
                        {
                            missingDependencies = true;
                            example = dep;
                            break;
                        }
                    }
                    if (!missingDependencies)
                    {
                        foreach (ModMeta.Dependency opDep in mod.OptionalDependencies)
                        {
                            if (ContainsOutdated(opDep, Mods))
                            {
                                missingDependencies = true;
                                example = opDep;
                                break;
                            }
                        }
                    }

                    if (missingDependencies)
                    {
                        Logger.Log("Removing " + mod.Name + " due to outdated or missing dependencies,\n" +
                            "such as " + example.Name + " version " + example.VersionString);
                        ToRemove.Add(mod);
                    }
                }
                Mods.RemoveAll(m => ToRemove.Contains(m));
            } while (ToRemove.Any());

            Logger.Log("Stage 3: Preparing the waiting list");
            foreach (ModMeta mod in Mods)
            {
                if (mod.OptionalDependencies.Any())
                {
                    waiting.Add(mod);
                    continue;
                }
                bool wait = false;
                foreach (ModMeta.Dependency opDep in mod.Dependencies)
                {
                    if (!Contains(opDep, loaded))
                    {
                        wait = true;
                        break;
                    }
                }
                if (wait)
                {
                    waiting.Add(mod);
                    continue;
                }
                LoadModFromMeta(mod);
            }

            Logger.Log("Stage 4: Navigating the dependency tree");
            List<ModMeta> loadedThisInteration = new();
            while (waiting.Any())
            {
                loadedThisInteration.Clear();
                foreach (ModMeta mod in waiting)
                {
                    foreach (ModMeta.Dependency dep in mod.Dependencies)
                    {
                        if (!Contains(dep, loaded))
                        {
                            // if deps are now loaded, load and remove from waiting list
                            continue;
                        }
                    }

                    bool waitingForDependencies = false;
                    foreach (ModMeta.Dependency opDep in mod.OptionalDependencies)
                    {
                        if (!Contains(opDep, loaded) && Contains(opDep, waiting))
                        {
                            // if dependency is unloaded, but is waiting to be loaded, wait for it
                            waitingForDependencies = true;
                            break;
                        }
                    }
                    if (waitingForDependencies)
                    {
                        continue;
                    }
                    LoadModFromMeta(mod);
                    loadedThisInteration.Add(mod);
                }
                waiting.RemoveAll(m => loadedThisInteration.Contains(m));
                if (!loadedThisInteration.Any())
                {
                    // if we don't load any mods, we have a circular dep, don't load any more
                    foreach (ModMeta item in waiting)
                    {
                        Logger.Log("Not loading " + item.Name + ": circular dependency!");
                    }
                    break;
                }
            }
            Logger.Log("Order finallized, Please enjoy the show!");
            // Add mod content
            // Load mods
            foreach (var mod in CodeMods)
                mod.Load();
            Logger.Log($"Finished pre-init loading - {Mods.Count} mods loaded; {CodeMods.Count} assemblies, {ModContentDirectories.Count} content directories, and {ModCampaignModels.Count} custom campaigns found.");
        }
        catch (Exception e)
        {
            if (Logger.Setup)
            {
                Logger.Log("Failed to pre-initialize!");
                Logger.Log(e);
            }
            throw;
        }
    }

    private static void LoadModFromMeta(ModMeta mod)
    {
        if (mod == QuintessentialModMeta)
            return;
        if (!string.IsNullOrWhiteSpace(mod.DLL))
        {
            string dllPath = mod.DLL;
            LoadModAssembly(mod, GetRemappedAssembly(dllPath, mod));
        }
        // Get mod content
        //  - Consider modded folders when fetching any content
        //  - Custom language files: vanilla stores in a big CSV, but for custom dialogue (and languages) we'll want seperate files (e.g. English.txt, French.txt)
        //  - Custom solitaires too?
        var content = Path.Combine(mod.PathDirectory, "Content");
        if (Directory.Exists(content))
            ModContentDirectories.Add(mod.PathDirectory);

        LoadModCampaigns(mod);

        loaded.Add(mod);
        Logger.Log($"Will load mod \"{mod.Name}\".");
    }

    private static void LoadModCampaigns(ModMeta mod)
    {
        var puzzles = Path.Combine(mod.PathDirectory, "Puzzles");
        if (Directory.Exists(puzzles))
        {
            if (!ModPuzzleDirectories.Contains(puzzles))
                ModPuzzleDirectories.Add(puzzles);
            // Look for name.campaign.yaml and name.journal.yaml files in the folder
            foreach (var item in Directory.GetFiles(puzzles))
            {
                string filename = Path.GetFileName(item);
                if (filename.EndsWith(".campaign.yaml"))
                {
                    CampaignModel c = DataSerializer.Deserialize<CampaignModel>(item);
                    Logger.Log($"Campaign \"{c.Title}\" ({c.Name}) has {c.Chapters.Count} chapters.");
                    c.Path = Path.GetDirectoryName(item);
                    ModCampaignModels.Add(c);
                }

                if (filename.EndsWith(".journal.yaml"))
                {
                    JournalModel c = DataSerializer.Deserialize<JournalModel>(item);
                    Logger.Log($"Journal \"{c.Title}\" has {c.Chapters.Count} chapters.");
                    foreach (var chapter in new List<JournalChapterModel>(c.Chapters))
                    {
                        if (chapter.Puzzles.Count != 5)
                        {
                            Logger.Log($"Journal chapter \"{chapter.Title}\" in \"{c.Title}\" has {chapter.Puzzles.Count} puzzles instead of 5; skipping chapter.");
                            c.Chapters.Remove(chapter);
                        }
                    }

                    if (c.Chapters.Count > 0)
                    {
                        c.Path = Path.GetDirectoryName(item);
                        ModJournalModels.Add(c);
                    }
                    else
                        Logger.Log($"Journal \"{c.Title}\" has no chapters, skipping.");
                }
            }
        }
    }

    public static void PostLoad()
    {
        Logger.Log("Starting post-init loading.");
        // Read mod save data
        PathModSaves = Path.Combine(class_161.method_402(), "ModSettings");
        Logger.Log($"Mod settings directory: \"{PathModSaves}\"");
        if (!Directory.Exists(PathModSaves))
            Directory.CreateDirectory(PathModSaves);
        foreach (var mod in CodeMods)
        {
            var savePath = Path.Combine(PathModSaves, mod.Meta.Name + ".yaml");
            if (File.Exists(savePath))
            {
                var settings = DataSerializer.Deserialize(savePath, mod.SettingsType);
                if (settings != null)
                {
                    mod.Settings = settings;
                    mod.ApplySettings();
                }
                else
                    Logger.Log("Loaded null settings for mod " + mod.Meta.Name);
            }
        }
        foreach (var mod in CodeMods)
            mod.PostLoad();
        Logger.Log("Finished post-init loading.");
    }

    public static void LoadPuzzleContent()
    {
        Logger.Log("Starting puzzle content loading.");
        foreach (var mod in CodeMods)
            mod.LoadPuzzleContent();

        Logger.Log("Loading campaigns and journals.");
        LoadCampaigns();
        LoadJournals();

        Logger.Log("Finished puzzle content loading.");
    }

    public static void Unload()
    {
        Logger.Log("Starting mod unloading.");
        foreach (var mod in CodeMods)
            mod.Unload();
        Logger.Log("Finished unloading.");
    }

    protected static void FindZipMod(string zip)
    {
        Logger.Log("Unzipping zip mod: " + zip);
        // Check that the zip exists
        if (!File.Exists(zip)) // Relative path?
            zip = Path.Combine(PathMods, zip);
        if (!File.Exists(zip)) // It just doesn't exist.
            return;

        var dest = Path.Combine(PathUnpackedMods, Path.GetFileNameWithoutExtension(zip));
        using (ZipFile file = new(zip))
            file.ExtractAll(dest);
        FindFolderMod(dest, zip);
    }

    protected static void FindFolderMod(string dir, string zipName = null)
    {
        // don't load zip mods again, ignore quintessential assets
        if (dir.EndsWith(quintAssetFolder) || (string.IsNullOrEmpty(zipName) && dir.EndsWith(zipExtractSuffix)))
            return;

        Logger.Log($"Finding mod in folder: \"{dir}\"");
        // Check that the folder exists
        if (!Directory.Exists(dir)) // Relative path?
            dir = Path.Combine(PathMods, dir);
        if (!Directory.Exists(dir)) // It just doesn't exist.
            return;

        // Look for a mod meta
        ModMeta meta;
        string metaPath = Path.Combine(dir, "quintessential.yaml");
        if (!File.Exists(metaPath))
            metaPath = Path.Combine(dir, "quintessential.yml");
        if (File.Exists(metaPath))
        {
            using StreamReader reader = new(metaPath);

            try
            {
                if (!reader.EndOfStream)
                {
                    meta = DataSerializer.Deserialize<ModMeta>(metaPath);
                    meta.Name = meta.Name.Trim().Replace(" ", "_");
                    meta.PathDirectory = dir;
                    if (!string.IsNullOrEmpty(zipName))
                        meta.PathArchive = zipName;
                    meta.PostParse();
                    Mods.Add(meta);
                    Logger.Log($"Queuing mod \"{meta.Name}\", version {meta.VersionString}.");
                }
            }
            catch (Exception e)
            {
                Logger.Log($"Failed parsing quintessential.yaml in {dir}: {e}");
            }
        }
        else
        {
            meta = new ModMeta
            {
                Name = "NoMetaMod__" + Path.GetFileName(dir),
                PathDirectory = dir
            };
            if (!string.IsNullOrEmpty(zipName))
                meta.PathArchive = zipName;
            meta.PostParse();
            Mods.Add(meta);
            Logger.Log($"Will load mod without metadata from \"{dir}\".");
        }
    }

    protected static void LoadModAssembly(ModMeta meta, Assembly asm)
    {
        Type[] types;
        try
        {
            try
            {
                types = asm.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(t => t != null).ToArray();
            }
        }
        catch (Exception e)
        {
            Logger.Log($"Failed reading assembly for {meta.Name}: {e}");
            Logger.Log(e);
            return;
        }

        foreach (var type in types)
        {
            if (typeof(QuintessentialMod).IsAssignableFrom(type) && !type.IsAbstract)
            {
                QuintessentialMod mod = (QuintessentialMod)type.GetConstructor(new Type[] { }).Invoke(new object[] { });
                mod.Meta = meta;
                Register(mod);
            }
        }
    }

    protected static void CleanupLegacyExtractedData()
    {
        string[] folders = Directory.GetDirectories(PathMods);
        foreach (var folder in folders)
            if (folder.EndsWith(zipExtractSuffix) || folder.EndsWith(quintAssetFolder))
                Directory.Delete(folder, true);
    }

    protected static void Register(QuintessentialMod mod)
    {
        CodeMods.Add(mod);
    }

    public static Assembly GetRemappedAssembly(string asmPath, ModMeta meta)
    {
        if (!string.IsNullOrEmpty(meta.Mappings))
        {
            // load mappings
            // load assembly def
            // remap
            // save in cache
            // load that
            //OpusMutatum.OpusMutatum.DoRemap();
        }
        return Assembly.LoadFrom(asmPath);
    }

    public static void LoadCampaigns()
    {
        AllCampaigns.Clear();

        VanillaCampaign = Campaigns.opusMagnum;
        ((patch_Campaign)(object)VanillaCampaign).QuintTitle = "Opus Magnum";
        AllCampaigns.Add(VanillaCampaign);

        foreach (var c in ModCampaignModels)
        {
            var campaign = new Campaign
            {
                chapters = new CampaignChapter[c.Chapters.Count]
            };

            ((patch_Campaign)(object)campaign).QuintTitle = c.Title;

            for (int j = 0; j < c.Chapters.Count; j++)
            {
                ChapterModel chapter = c.Chapters[j];
                campaign.chapters[j] = new CampaignChapter(
                    Translations.Translate(chapter.Title),
                    Translations.Translate(chapter.Subtitle),
                    Translations.Translate(chapter.Place),
                    chapter.Background != null ? AssetLoaderHelper.LoadTexture(chapter.Background) : Campaigns.opusMagnum.chapters[j].background,
                    Campaigns.opusMagnum.chapters[j].lockedIcon,
                    Campaigns.opusMagnum.chapters[j].unlockedIcon,
                    Campaigns.opusMagnum.chapters[j].hoverIcon,
                    Campaigns.opusMagnum.chapters[j].gemIcon,
                    Campaigns.opusMagnum.chapters[j].buttonOffset,
                    (ChapterAlignment)1
                );

                foreach (var entry in chapter.Entries)
                {
                    UnlockRequirement requirement = string.IsNullOrEmpty(entry.Requires) ? (UnlockRequirement)new UnlockReqNothing() : new UnlockReqCompleteCampaignItem(entry.Requires);

                    var lower = entry.Type.ToLowerInvariant();
                    CampaignItem cItem;
                    switch (lower)
                    {
                        case "puzzle":
                            {
                                if (!TryLoadPuzzle(c.Path, entry.Puzzle, c.Title, out var puzzle))
                                    continue;

                                puzzle.puzzleId = entry.ID;
                                // ensure all inputs/outputs have names
                                foreach (PuzzleInputOutput io in puzzle.inputs.Union(puzzle.outputs))
                                {
                                    if (!io.molecule.displayName.HasValue())
                                    {
                                        io.molecule.displayName = Translations.Translate("Molecule");
                                    }
                                }

                                // TODO: optimize
                                cItem = AddEntryToCampaign(campaign, j, entry.ID, Translations.Translate(entry.Title), (CampaignItemType)0, MaybeHelper.empty, puzzle, Assets.musicTracks.field_972, Assets.sounds.field_1832, requirement, entry.NoStoryPanel);
                                Array.Resize(ref Puzzles.campaignPuzzles, Puzzles.campaignPuzzles.Length + 1);
                                Puzzles.campaignPuzzles[Puzzles.campaignPuzzles.Length - 1] = puzzle;
                                break;
                            }
                        case "solitaire":
                            {
                                cItem = new(entry.ID, Translations.Translate("Sigmar's Garden"), (CampaignItemType)3, MaybeHelper.empty, requirement, Assets.musicTracks.field_970, Assets.sounds.field_1830, campaign);
                                campaign.chapters[j].campaignItems.Add(cItem);
                                break;
                            }
                        default:
                            Logger.Log($"Campaign entry in {c.Name} has unknown type {entry.Type}, skipping");
                            continue;
                    }

                    patch_CampaignItem conv = (patch_CampaignItem)(object)cItem;

                    // todo: fix this

                    // probably not great to reload the images every time, in the case that a campaign uses the same image on every puzzle?
                    // but these are small, and we can definitely handle the case where every puzzle has a unique icon
                    if (!string.IsNullOrWhiteSpace(entry.Icon))
                        conv.Icon = AssetLoaderHelper.LoadTexture(entry.Icon);
                    if (!string.IsNullOrWhiteSpace(entry.IconSmall))
                        conv.IconSmall = AssetLoaderHelper.LoadTexture(entry.IconSmall);
                }
            }

            for (int index = 0; index < campaign.chapters.Length; ++index)
                campaign.chapters[index].chapterNumber = index;

            AllCampaigns.Add(campaign);
        }
    }

    public static void LoadJournals()
    {
        AllJournals.Clear();

        VanillaJournal = JournalVolumes.volumes.ToList();
        AllJournals.Add(VanillaJournal);

        foreach (JournalModel journal in ModJournalModels)
        {
            // todo: allow custom journal images?

            List<JournalVolume> volumes = journal.Chapters.Select(chapter =>
                new JournalVolume
                {
                    issueName = Translations.Translate(chapter.Title),
                    flavorText = Translations.Translate(chapter.Description),
                    puzzles = GetJournalPuzzles(chapter,journal)
                }).ToList();

            // add journal puzzles to list of puzzles
            foreach (JournalVolume volume in volumes)
            {
                int l = Puzzles.campaignPuzzles.Length;
                Array.Resize(ref Puzzles.campaignPuzzles, l + volume.puzzles.Length /* should always be 5, but better safe than sorry. */  );
                int i = 0;
                foreach (var puzzle in volume.puzzles)
                {
                    Puzzles.campaignPuzzles[l + i] = puzzle;
                    i++;
                } // this is a little bit of a patchy method of doing it, i'm not sure the exact form the journal puzzle array takes and i'm a little bit tired from testing it, also taken from icwass RMC journal code
            }
            // log journals
            foreach (JournalVolume jv in volumes)
            {
                Logger.Log($"Journal {jv.issueName} has {jv.puzzles.Length} puzzles");
            }
            AllJournals.Add(volumes);
        }
    }
    public static Puzzle[] GetJournalPuzzles(JournalChapterModel chapter, JournalModel journal) {
        IEnumerable<Puzzle> puzzles = new List<Puzzle>();
        foreach( var puzzleName in chapter.Puzzles) {
            Puzzle p = TryLoadPuzzle(journal.Path, puzzleName, journal.Title, out var puzzle) ? puzzle : new Puzzle();
            puzzles.Concat(p);
        }
        return puzzles.ToArray();
    }

    private static bool TryLoadPuzzle(string basePath, string puzzleName, string campaignTitle, out Puzzle puzzle)
    {
        try
        {
            string baseName = Path.Combine(basePath, puzzleName);
            if (File.Exists(baseName + ".puzzle")) {
                puzzle = Puzzle.LoadFromFile(baseName + ".puzzle");
            } else if (File.Exists(baseName + ".puzzle.jsonc")) {
                puzzle = PuzzleModel.FromModel(DataSerializer.Deserialize<PuzzleModel>(baseName + ".puzzle.jsonc"));
            } else if (File.Exists(baseName + ".puzzle.json")) {
                puzzle = PuzzleModel.FromModel(DataSerializer.Deserialize<PuzzleModel>(baseName + ".puzzle.json"));
            } else if (File.Exists(baseName + ".puzzle.yaml")) {
                puzzle = PuzzleModel.FromModel(DataSerializer.Deserialize<PuzzleModel>(baseName + ".puzzle.yaml"));
            } else {
                Logger.Log($"Puzzle \"{puzzleName}\" from \"{campaignTitle}\" doesn't exist, ignoring");
                puzzle = null;
                return false;
            }

            // even if it was loaded from a vanilla format puzzle file, it was included in a mod and may rely on modded behaviour
            // these are never saved over and could have been modified directly by the campaign mod, so this is safe
            ((patch_Puzzle)(object)puzzle).IsModdedPuzzle = true;

            return true;
        }
        catch (Exception e)
        {
            Logger.Log($"Exception loading puzzle \"{puzzleName}\" from \"{campaignTitle}\", ignoring");
            Logger.Log(e);
            puzzle = null;
            return false;
        }
    }

    public static void CheckCampaignReload()
    {
        if (QuintessentialSettings.Instance.HotReloadCampaigns.Pressed() && GameLogic.instance.GetLastScreen() is PuzzleSelectScreen)
        {
            Logger.Log("Reloading campaigns and journals!");

            ModPuzzleDirectories.Clear();
            ModCampaignModels.Clear();
            ModJournalModels.Clear();

            Campaigns.opusMagnum = VanillaCampaign;
            Campaigns.campaigns[0] = VanillaCampaign;
            JournalVolumes.volumes = VanillaJournal.ToArray();
            patch_PuzzleSelectScreen.ResetPosition();
            patch_JournalScreen.ResetPosition();

            foreach (ModMeta mod in Mods)
                if (mod != QuintessentialModMeta)
                    LoadModCampaigns(mod);

            LoadCampaigns();
            LoadJournals();
            UI.InstantCloseScreen();
            UI.OpenScreen(new PuzzleSelectScreen());
        }
    }

    private static CampaignItem AddEntryToCampaign(
            Campaign campaign,
            int chapter,
            string itemId,
            LocString itemName,
            CampaignItemType itemType,
            Maybe<Tip> puzzleTip,
            Maybe<Puzzle> puzzle,
            MusicTrack musicTrack,
            Sound fanfare,
            UnlockRequirement requirement,
            bool noStoryPanel
    )
    {
        if (puzzle.HasValue())
        {
            //puzzle.method_1087().field_2767 = entryTitle;
            puzzle.GetValue().puzzleTip = puzzleTip;
        }
        CampaignItem campaignItem = new(itemId, itemName, itemType, puzzle, requirement, musicTrack, fanfare, campaign);
        campaign.chapters[chapter].campaignItems.Add(campaignItem);
        // no cutscene to see here
        if (noStoryPanel)
            campaignItem.vignette = MaybeHelper.empty;

        return campaignItem;
    }

    internal static void DumpVanillaPuzzles()
    {
        string outDir = Path.Combine(PathModSaves, "Quintessential", "DumpedPuzzles");
        Directory.CreateDirectory(outDir);
        foreach (var p in Puzzles.campaignPuzzles)
        {
            PuzzleModel m = PuzzleModel.FromPuzzle(p);
            DataSerializer.Serialize(Path.Combine(outDir, m.ID + ".puzzle.jsonc"), m, true);
        }
        foreach (var volume in JournalVolumes.volumes)
        {
            foreach (var p in volume.puzzles) {
                PuzzleModel m = PuzzleModel.FromPuzzle(p);
                DataSerializer.Serialize(Path.Combine(outDir, "X" + m.ID + ".puzzle.jsonc"), m, true);
            }
        }
        Logger.Log($"Dumped puzzles to {outDir}");
        UI.OpenScreen(new NoticeScreen("Puzzle Dumping", $"Saved puzzles to \"{outDir.Replace('\\', '/')}\""));
    }

    internal static void DumpAtomSprites()
    {
        string outDir = Path.Combine(PathModSaves, "Quintessential", "DumpedAtomSprites");
        Directory.CreateDirectory(outDir);
        foreach (AtomType atomType in AtomTypes.atoms)
        {
            RenderTargetHandle v = RenderAtomToTarget(atomType);
            Renderer.PngFromTexture(v.GetTarget().renderedTexture).Save(Path.Combine(outDir, ((patch_AtomType)(object)atomType).QuintAtomType.Replace(":", "_") + ".png"));
        }
        Logger.Log($"Dumped atom sprites to {outDir}");
        UI.OpenScreen(new NoticeScreen("Sprite Dumping", $"Saved atom sprites to \"{outDir.Replace('\\', '/')}\""));
    }

    internal static RenderTargetHandle RenderAtomToTarget(AtomType type)
    {
        RenderTargetHandle renderTargetHandle = new RenderTargetHandle();
        Bounds2 bounds = Bounds2.CenteredOn(HexGrid.standardGrid.ToWorldCoords(new HexIndex(0, 0), Vector2.Zero), HexGrid.standardGrid.hexSize.X, HexGrid.standardGrid.hexSize.Y * 1.3f);
        Index2 size = bounds.Size.CeilingToInt() + new Index2(20 * 2, 20 * 2);
        Vector2 pos = size.ToVector2() / 2 / 1f - bounds.Center;
        pos.Y = -pos.Y;
        renderTargetHandle.targetSize = size;
        RenderTarget class95 = renderTargetHandle.GetTarget(out var flag);
        if (flag)
        {
            using (class_226.method_597(class95, Matrix4.GetScale(new Vector3(1, -1, 1))))
            {
                class_226.method_600(Color.Transparent);
                Editor.RenderAtom(type, pos, 1, 1, 1, 1, -21, 0, Assets.textures.white, null, false);
            }
        }

        return renderTargetHandle;
    }
}
