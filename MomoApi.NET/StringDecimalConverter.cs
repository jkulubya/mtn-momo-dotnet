using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MomoApi.NET
{
    public class StringDecimalConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if(value == null) return;
            
            if (value is decimal d)
            {
                writer.WriteValue(d.ToString(CultureInfo.InvariantCulture));
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            
            if (token.Type == JTokenType.String)
            {
                return Decimal.Parse(token.ToString(), CultureInfo.InvariantCulture);
            }
            
            throw new JsonSerializationException($"Unexpected token type {token.Type}");
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal) || objectType == typeof(decimal?));
        }
    }
}