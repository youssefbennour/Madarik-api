using System.Text.Json.Serialization;

namespace Madarik.Madarik.Data.Topic;

public class Topic
{
#pragma warning disable CS8618, CS9264
    private Topic()
#pragma warning restore CS8618, CS9264
    {
        
    }
    public Topic(string name, List<Chapter> chapters)
    {
        Id = Guid.NewGuid();
        Name = name;
        Chapters = chapters;
    }

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("chapters")]
    public List<Chapter> Chapters { get; set; }
} 