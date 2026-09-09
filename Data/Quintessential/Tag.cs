using Quintessential.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Quintessential;
public abstract class Tag {

    public readonly Identifier Id;
    public readonly bool IsTable;
    public readonly HashSet<Identifier> Entries;
    public readonly Dictionary<Identifier, Identifier?> TableEntries;

    public Tag(Identifier id, bool isTable = false) {
        Id = id;
        IsTable = isTable;
        if (isTable)    // Do not init unused fields
            TableEntries = [];
        else Entries = [];
        if (IsTable && !id.name.StartsWith('$')) throw new Exception("Tag-table names must start with '$', found violation in: " + Id);
        else if (!IsTable && id.name.StartsWith('$')) throw new Exception("Non Tag-table names must not start with '$', found violation in: " + Id);
    }

    public static void DumpTags<T>(Dictionary<Identifier, T> tags, string fileName) where T : Tag { // "tags.jsonc"
        DataSerializer.SetMultilineFormat(true);
        string outDir = Path.Combine(QuintessentialLoader.PathModSaves, "Quintessential", "DumpedTags");
        Directory.CreateDirectory(outDir);
        tags.Serialize(Path.Combine(outDir, fileName));
    }

    public bool HasEntry(Identifier id) {
        return IsTable ? TableEntries.ContainsKey(id) : Entries.Contains(id);
    }
    public bool HasEntry(Identifier id, out Identifier? mapped) {
        mapped = IsTable && TableEntries.TryGetValue(id, out Identifier? value) ? value : null;
        return IsTable ? TableEntries.ContainsKey(id) : Entries.Contains(id);
    }
    public Identifier? GetMapped(Identifier id) {
        return IsTable && TableEntries.TryGetValue(id, out Identifier? value) ? value : null;
    }

    public void Add(Identifier id, Identifier? mapped = null) {
        if (IsTable ^ mapped != null) throw new Exception("When calling Tag.Add() the 'mapped' value should be null if and only if the tag isn't a table-tag.");
        if (IsTable) {
            TableEntries.Add(id, mapped);
        } else Entries.Add(id);
    }
    public void Remove(Identifier id) {
        if (IsTable) TableEntries.Remove(id);
        else Entries.Remove(id);
    }
}

public abstract class TagJsonConverter<T> : JsonConverter<Dictionary<Identifier, T>> where T : Tag {
    private readonly Dictionary<Identifier, T> GlobalTags;
    private readonly Func<Identifier, bool, T> CtorForType;
    protected TagJsonConverter(Dictionary<Identifier, T> GlobalTags, Func<Identifier, bool, T> CtorForType) {
        this.GlobalTags = GlobalTags;
        this.CtorForType = CtorForType;
    }

    public override Dictionary<Identifier, T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType != JsonTokenType.StartObject) {
            if (reader.TokenType == JsonTokenType.PropertyName) {
                ReadSingle(ref reader, GlobalTags);
                return GlobalTags;
            }
            throw new FormatException("Tags file must begin with '{'");
        }
        reader.Read();
        while (reader.TokenType != JsonTokenType.EndObject) {
            ReadSingle(ref reader, GlobalTags);
        }
        reader.Read();
        return GlobalTags;
    }
    public void ReadSingle(ref Utf8JsonReader reader, Dictionary<Identifier, T> tagSet) {

        string id = reader.GetString();
        reader.Read();
        bool isTable = reader.TokenType == JsonTokenType.StartObject;
        if (!tagSet.TryGetValue(id, out var toEdit)) {
            toEdit = CtorForType.Invoke(id, isTable);
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
            throw new JsonException($"Tag '{id}' contains unecpected characters.");
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<Identifier, T> tags, JsonSerializerOptions options) {
        writer.WriteStartObject();
        foreach (var tag in tags) {
            if (tag.Key != tag.Value.Id) throw new JsonException($"Tag id mismatch found, '{tag.Value.Id}' stored as '{tag.Key}'");

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
