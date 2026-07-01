using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Quintessential.Serialization {
    public static class DataSerializer {
        public static T Deserialize<T>(string filePath) {
            return (T)Deserialize(filePath, typeof(T));
        }
        public static object Deserialize(string filePath, Type type) {
            string filename = Path.GetFileName(filePath);

            if (filename.EndsWith(".yaml")) {
                using StreamReader reader = new(filePath);
                return YamlHelper.Deserializer.Deserialize(reader, type);
            }

            if (filename.EndsWith(".json")) {
                DataContractJsonSerializer jsonSerializer = new(type);
                using FileStream fileStream = new(filePath, FileMode.Open);
                return jsonSerializer.ReadObject(fileStream);
            }

            if (filename.EndsWith(".jsonc")) {
                string jsonCData = File.ReadAllText(filePath, Encoding.UTF8);
                string data = ToJson(jsonCData);

                var byteArray = Encoding.UTF8.GetBytes(data);
                var memoryStream = new MemoryStream(byteArray);
                DataContractJsonSerializer jsonSerializer = new(type);
                return jsonSerializer.ReadObject(memoryStream);
            }

            throw new SerializationException("Invalid file extension at: " + filePath);
        }

        public static void Serialize<T>(string filePath, T data, bool multilineFormat = false) {
            string filename = Path.GetFileName(filePath);

            if (filename.EndsWith(".yaml")) {
                string serializedData = YamlHelper.Serializer.Serialize(data);
                File.WriteAllText(filePath, serializedData);
                return;
            }

            if (filename.EndsWith(".json") || filename.EndsWith(".jsonc")) {

                var jsonSerializer = new DataContractJsonSerializer(typeof(T));
                using FileStream fileStream = new(filePath, FileMode.OpenOrCreate);
                using var writer = JsonReaderWriterFactory.CreateJsonWriter(fileStream, Encoding.UTF8, true, multilineFormat);
                jsonSerializer.WriteObject(writer, data);
                writer.Flush();
                return;
            }

            throw new SerializationException("Invalid file extension while serializing: " + filePath);
        }

        private static string ToJson(string jsoncData) {
            StringBuilder jsonData = new();

            bool isComment = false;
            bool isMultiLineComment = false;
            bool stringData = false;
            bool addedComma = false;
            string c = "";

            for (int i = 0; i < jsoncData.Length; i++) {
                string c0 = c;
                c = jsoncData.Substring(i,1);

                if (stringData) {
                    if (c == "\"") stringData = false;
                    jsonData.Append(c);
                    continue;
                }
                if (isComment) {
                    if (c == "\n") isComment = false;
                    continue;
                }
                if (isMultiLineComment) {
                    if (c0 == "*" && c == "/") isMultiLineComment = false;
                    continue;
                }

                if (addedComma && (c == "]" || c == "}")) jsonData.Remove(jsonData.Length - 1, 1);
                if (c != " " && c != "\n" && c != "\r" && c != "\t") {
                    jsonData.Append(c);
                    addedComma = c == ",";
                }

                if (c == "\"") {
                    stringData = true;
                    addedComma = false;
                } else if (c0 == "/" && c == "/") {
                    isComment = true;
                    jsonData.Remove(jsonData.Length - 2, 2);
                } else if (c0 == "/" && c == "*") {
                    isMultiLineComment = true;
                    jsonData.Remove(jsonData.Length - 2, 2);
                }
            }
            return jsonData.ToString();
        }

        public class SerializationException : Exception {
            public SerializationException(string message) : base(message) { }
            public SerializationException(string message, Exception innerException) : base(message, innerException) { }

        }
    }
}
