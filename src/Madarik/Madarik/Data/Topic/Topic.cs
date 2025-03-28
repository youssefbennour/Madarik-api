using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Madarik.Madarik.Data.Topic;

public class Topic
{
#pragma warning disable CS8618, CS9264
    private Topic()
#pragma warning restore CS8618, CS9264
    {
        
    }
    public Topic(Guid id, string name, List<Chapter> chapters)
    {
        Id = id;
        Name = name;
        Chapters = chapters;
    }

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("difficulty")]
    public required string Difficulty { get; set; }

    [JsonPropertyName("chapters")]
    public List<Chapter> Chapters { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public Quiz? Quiz { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public TopicQuizAnswers QuizAnswers { get; set; } = new();
    
    [System.Text.Json.Serialization.JsonIgnore]
    public bool IsCompleted { get; set; }

    [Newtonsoft.Json.JsonIgnore]
#pragma warning disable S3358
    public double Progress => IsCompleted ? 100 :  Chapters.Count(m => m.IsCompleted) * 100 / (Chapters.Count+1);
#pragma warning restore S3358
    [Newtonsoft.Json.JsonIgnore]
    public int ChaptersCount => Chapters.Count;
    
    
} 