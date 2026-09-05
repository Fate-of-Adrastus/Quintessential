using Quintessential.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Quintessential;

public class AtomTag {
    public static Dictionary<Identifier, AtomTag> AtomTags = [];

    public readonly Identifier Id;
    public readonly bool IsTable;
    public readonly HashSet<Identifier> Entries;
    public readonly Dictionary<Identifier, Identifier?> TableEntries;

    public AtomTag(Identifier id, bool isTable = false) {
        Id = id;
        IsTable = isTable;
        if (isTable)    // Do not init unused fields
            TableEntries = [];
        else Entries = [];
        if (IsTable && !id.name.StartsWith('$')) throw new Exception("Tag-table names must start with '$', found violation in: " + Id);
        else if (!IsTable && id.name.StartsWith('$')) throw new Exception("Non Tag-table names must not start with '$', found violation in: " + Id);
    }

    public static void DumpTags() {
        DataSerializer.SetMultilineFormat(true);
        string outDir = Path.Combine(QuintessentialLoader.PathModSaves, "Quintessential", "DumpedTags");
        Directory.CreateDirectory(outDir);
        AtomTags.Serialize(Path.Combine(outDir, "atomTags.jsonc"));
    }


    public bool HasAtom(Identifier atomId) {
        return IsTable ? TableEntries.ContainsKey(atomId) : Entries.Contains(atomId);
    }
    public bool HasAtom(AtomType atom) {
        return IsTable ? TableEntries.ContainsKey(atom.QuintAtomType) : Entries.Contains(atom.QuintAtomType);
    }
    public bool HasAtom(Atom atom) {
        return IsTable ? TableEntries.ContainsKey(atom.atomType.QuintAtomType) : Entries.Contains(atom.atomType.QuintAtomType);
    }
    public bool HasAtom(AtomReference atom ) {
        return IsTable ? TableEntries.ContainsKey(atom.atomType.QuintAtomType) : Entries.Contains(atom.atomType.QuintAtomType);
    }
    public bool HasAtom(Identifier atomId, out Identifier? mapped) {
        mapped = IsTable && TableEntries.TryGetValue(atomId, out Identifier? value) ? value : null;
        return IsTable ? TableEntries.ContainsKey(atomId) : Entries.Contains(atomId);
    }
    public bool HasAtom(AtomType atom, out Identifier? mapped) {
        mapped = IsTable && TableEntries.TryGetValue(atom.QuintAtomType, out Identifier? value) ? value : null;
        return IsTable ? TableEntries.ContainsKey(atom.QuintAtomType) : Entries.Contains(atom.QuintAtomType);
    }
    public bool HasAtom(Atom atom, out Identifier? mapped) {
        mapped = IsTable && TableEntries.TryGetValue(atom.atomType.QuintAtomType, out Identifier? value) ? value : null;
        return IsTable ? TableEntries.ContainsKey(atom.atomType.QuintAtomType) : Entries.Contains(atom.atomType.QuintAtomType);
    }
    public bool HasAtom(AtomReference atom, out Identifier? mapped) {
        mapped = IsTable && TableEntries.TryGetValue(atom.atomType.QuintAtomType, out Identifier? value) ? value : null;
        return IsTable ? TableEntries.ContainsKey(atom.atomType.QuintAtomType) : Entries.Contains(atom.atomType.QuintAtomType);
    }
    public Identifier? GetMapped(Identifier atomId) {
        return IsTable && TableEntries.TryGetValue(atomId, out Identifier? value) ? value : null;
    }
    public Identifier? GetMapped(AtomType atom) {
        return IsTable && TableEntries.TryGetValue(atom.QuintAtomType, out Identifier? value) ? value : null;
    }
    public Identifier? GetPair(Atom atom) {
        return IsTable && TableEntries.TryGetValue(atom.atomType.QuintAtomType, out Identifier? value) ? value : null;
    }
    public Identifier? GetMapped(AtomReference atom) {
        return IsTable && TableEntries.TryGetValue(atom.atomType.QuintAtomType, out Identifier? value) ? value : null;
    }

    internal class AtomTagJsonConverter : JsonConverter<Dictionary<Identifier, AtomTag>> {
        // TODO: make the Tag Dictionary configurable

        public override Dictionary<Identifier, AtomTag> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            if (reader.TokenType != JsonTokenType.StartObject) {
                if (reader.TokenType == JsonTokenType.PropertyName) {
                    ReadSingleAtomTag(ref reader, AtomTags);
                    return AtomTags;
                }
                throw new FormatException("Atom tag must begin with '{'");
            }
            reader.Read();
            while (reader.TokenType != JsonTokenType.EndObject) {
                ReadSingleAtomTag(ref reader, AtomTags);
            }
            reader.Read();
            return AtomTags;
        }
        public static void ReadSingleAtomTag(ref Utf8JsonReader reader, Dictionary<Identifier,AtomTag> tagSet) {

            string id = reader.GetString();
            reader.Read();
            bool isTable = reader.TokenType == JsonTokenType.StartObject;
            if (!tagSet.TryGetValue(id, out var toEdit)) {
                toEdit = new AtomTag(id, isTable);
                tagSet[id] = toEdit;
            }
            if (reader.TokenType == JsonTokenType.StartArray) {
                if (toEdit.IsTable) throw new JsonException($"Attempted to edit tag-table '{toEdit.Id}' as an non table");

                reader.Read();
                while (reader.TokenType != JsonTokenType.EndArray) {
                    string element = reader.GetString();
                    if (element.StartsWith('-')) {
                        if (toEdit.Entries.Contains(element[1..])) toEdit.Entries.Remove(element[1..]);
                    } else {
                        toEdit.Entries.Add(element);
                    }
                    reader.Read();
                }
                reader.Read();
            } else if (isTable) {
                if (!toEdit.IsTable) throw new JsonException($"Attempted to edit a non tag-table '{toEdit.Id}' as one.");

                reader.Read();
                while (reader.TokenType != JsonTokenType.EndObject) {
                    string element = reader.GetString();
                    reader.Read();
                    string mapped = null;
                    if (reader.TokenType == JsonTokenType.String) mapped = reader.GetString();
                    reader.Read();

                    if (mapped == null) {
                        if (toEdit.TableEntries.ContainsKey(element)) toEdit.TableEntries.Remove(element);
                    } else {
                        toEdit.TableEntries[element] = mapped;
                    }
                }
                reader.Read();
            } else
                throw new JsonException($"Atom tag '{id}' contains unecpected characters.");
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<Identifier, AtomTag> atomTags, JsonSerializerOptions options) {
            writer.WriteStartObject();
            foreach (var tag in atomTags) {
                if (tag.Key != tag.Value.Id) throw new JsonException($"Atom Tag id mismatch found, '{tag.Value.Id}' stored as '{tag.Key}'");

                writer.WritePropertyName(tag.Key);
                if (tag.Value.IsTable) {

                    writer.WriteStartObject();
                    foreach (var entry in tag.Value.TableEntries) {
                        writer.WriteString(entry.Key, entry.Value);
                    }
                    writer.WriteEndObject();
                } else { 

                    writer.WriteStartArray();
                    foreach (var entry in tag.Value.Entries) {
                        writer.WriteStringValue(entry);
                    }
                    writer.WriteEndArray();
                }
            }
            writer.WriteEndObject();
        }
    }
}
