using System.Text.Json.Serialization;

namespace Madarik.Madarik.Data.Topic;

public class TopicQuizAnswers
{
    [JsonPropertyName("answers")]
    public List<TopicQuizAnswer> Answers { get; set; } = new();
}

public class TopicQuizAnswer
{
    [JsonPropertyName("questionId")]
    public required Guid QuestionId { get; set; }

    [JsonPropertyName("answerId")]
    public required Guid AnswerId { get; set; }

    [JsonPropertyName("isCorrect")]
    public required bool IsCorrect { get; set; }

    [JsonPropertyName("submittedAt")]
    public required DateTimeOffset SubmittedAt { get; set; }
} 