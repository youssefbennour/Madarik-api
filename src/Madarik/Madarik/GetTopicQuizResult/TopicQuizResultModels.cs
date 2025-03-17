using System.Text.Json.Serialization;

namespace Madarik.Madarik.GetTopicQuizResult;

public class TopicQuizResultResponse
{
    [JsonPropertyName("numberOfQuestions")]
    public required int NumberOfQuestions { get; set; }

    [JsonPropertyName("numberOfCorrectAnswers")]
    public required int NumberOfCorrectAnswers { get; set; }

    [JsonPropertyName("score")]
    public required double Score { get; set; }
} 