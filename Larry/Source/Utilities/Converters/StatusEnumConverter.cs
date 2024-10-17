using Larry.Source.Enums;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Larry.Source.Utilities.Converters
{
    public class StatusEnumConverter : JsonConverter<StatusEnum>
    {
        /// <summary>
        /// Reads a JSON string and converts it into a <see cref="StatusEnum"/> value.
        /// </summary>
        /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
        /// <param name="typeToConvert">The type of object to convert (in this case, <see cref="StatusEnum"/>).</param>
        /// <param name="options">Options for serialization.</param>
        /// <returns>A <see cref="StatusEnum"/> value.</returns>
        /// <exception cref="JsonException">Thrown if the JSON is not in the expected format.</exception>
        public override StatusEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var enumValue = reader.GetString();

            return enumValue?.ToUpper() switch
            {
                "UP" => StatusEnum.Up,
                "DOWN" => StatusEnum.Down,
                _ => throw new JsonException("Invalid status value")
            };
        }

        /// <summary>
        /// Writes a <see cref="StatusEnum"/> value as a JSON string.
        /// </summary>
        /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
        /// <param name="value">The <see cref="StatusEnum"/> value to serialize.</param>
        /// <param name="options">Options for serialization.</param>
        public override void Write(Utf8JsonWriter writer, StatusEnum value, JsonSerializerOptions options)
        {
            var stringValue = value switch
            {
                StatusEnum.Up => "UP",
                StatusEnum.Down => "DOWN",
                _ => throw new JsonException("Invalid status value")
            };

            writer.WriteStringValue(stringValue);
        }
    }
}
