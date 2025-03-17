using System.Text.Json.Serialization;

namespace Madarik.Madarik.SubmitTopicQuiz;

public class TopicQuizAnswerSubmission
{
    [JsonPropertyName("questionId")]
    public required Guid QuestionId { get; set; }

    [JsonPropertyName("answerId")]
    public required Guid AnswerId { get; set; }
}

public class TopicQuizAnswerResponse
{
    [JsonPropertyName("isCorrect")]
    public required bool IsCorrect { get; set; }

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }
} 