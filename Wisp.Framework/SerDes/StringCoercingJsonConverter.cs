// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wisp.Framework.SerDes;

public class StringCoercingJsonConverter : JsonConverter<Dictionary<string, string>>
{
    public override Dictionary<string, string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dict = new Dictionary<string, string>();

        using var doc = JsonDocument.ParseValue(ref reader);
        foreach (var prop in doc.RootElement.EnumerateObject())
        {
            if (prop.Value.ValueKind == JsonValueKind.String)
            {
                dict[prop.Name] = prop.Value.GetString() ?? string.Empty;
            } 
            else if (prop.Value.ValueKind == JsonValueKind.Number 
                     || prop.Value.ValueKind == JsonValueKind.True 
                     || prop.Value.ValueKind == JsonValueKind.False)
            {
                dict[prop.Name] = prop.Value.ToString();
            }
            else
            {
                dict[prop.Name] = prop.Value.GetRawText();
            }
        }

        return dict;
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<string, string> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var kv in value)
        {
            writer.WriteString(kv.Key, kv.Value);
        }
        writer.WriteEndObject();
    }
}