using Larry.Source.Classes.MCP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Larry.Source.Utilities.Converters
{
    public class IgnoreNullsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true; 
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            RemoveNullsRecursively(jObject);
            return jObject.ToObject(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        private void RemoveNullsRecursively(JObject jObject)
        {
            foreach (var property in jObject.Properties().ToList())
            {
                if (property.Value.Type == JTokenType.Null)
                {
                    property.Remove();
                }
                else if (property.Value.Type == JTokenType.Object)
                {
                    RemoveNullsRecursively((JObject)property.Value);
                }
            }
        }
    }
}
