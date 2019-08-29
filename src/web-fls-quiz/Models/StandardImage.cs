using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WebFlsQuiz.Models
{
    public class StandardImage
    {
        public int Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public ImageType ImageType { get; set; }

        public string ImageBase64 { get; set; }
    }
}
