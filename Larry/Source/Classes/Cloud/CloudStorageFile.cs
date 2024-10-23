using System.Text.Json.Serialization;

namespace Larry.Source.Classes.Cloud
{
    public class CloudStorageFile
    {
        [JsonPropertyName("uniqueFilename")]
        public string UniqueFilename { get; set; }
        [JsonPropertyName("filename")]
        public string Filename { get; set; }
        [JsonPropertyName("hash")]
        public string Hash { get; set; }
        [JsonPropertyName("hash256")]
        public string Hash256 { get; set; }
        [JsonPropertyName("length")]
        public long Length { get; set; }
        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }
        [JsonPropertyName("uploaded")]
        public string Uploaded { get; set; }
        [JsonPropertyName("storageType")]
        public string StorageType { get; set; }
        [JsonPropertyName("doNotCache")]
        public bool DoNotCache { get; set; }
    }
}
