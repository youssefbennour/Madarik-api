using System.Text.Json.Serialization;

namespace Madarik.Madarik.Data.Topic;

public class Chapter
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("articles")] 
    public List<Article> Articles { get; set; } = new();

    [JsonPropertyName("quiz")]
    public Quiz? Quiz { get; set; }
} 