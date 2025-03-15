using System.Text.Json.Serialization;

namespace Madarik.Madarik.Data.Topic;

public class Article
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("url")]
    public required string Url { get; set; }

    [JsonPropertyName("author")]
    public string? Author { get; set; }
} 