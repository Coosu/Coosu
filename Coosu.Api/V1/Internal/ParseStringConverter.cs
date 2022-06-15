using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Coosu.Api.V1.Internal;

internal class ParseStringConverter : JsonConverter<long?>
{
    public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetInt64();
        return long.Parse(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}