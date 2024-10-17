using Larry.Source.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Larry.Source.Utilities.Converters
{
    public class ActionsEnumListConverter : JsonConverter<List<ActionsEnum>>
    {
        /// <summary>
        /// Reads a JSON array of strings and converts them into a list of <see cref="ActionsEnum"/> values.
        /// </summary>
        public override List<ActionsEnum> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = new List<ActionsEnum>();

            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Expected StartArray token");
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }

                string enumValue = reader.GetString();
                if (Enum.TryParse<ActionsEnum>(enumValue, ignoreCase: true, out var action))
                {
                    result.Add(action);
                }
                else
                {
                    throw new JsonException($"Invalid action value: {enumValue}");
                }
            }

            return result;
        }

        /// <summary>
        /// Writes a list of <see cref="ActionsEnum"/> values as a JSON array of strings.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, List<ActionsEnum> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var action in value)
            {
                writer.WriteStringValue(action.ToString());
            }

            writer.WriteEndArray();
        }
    }
}
