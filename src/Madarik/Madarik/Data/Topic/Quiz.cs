using System.Text.Json.Serialization;

namespace Madarik.Madarik.Data.Topic;

public class Quiz
{
    [JsonPropertyName("questions")]
    public required List<QuizQuestion> Questions { get; set; }
}

public class QuizQuestion
{
    [JsonPropertyName("question")]
    public required string Question { get; set; }

    [JsonPropertyName("possibleAnswers")]
    public required List<PossibleAnswer> PossibleAnswers { get; set; }

    [JsonPropertyName("explanation")]
    public required string Explanation { get; set; }
}

public class PossibleAnswer
{
    [JsonPropertyName("answer")]
    public required string Answer { get; set; }

    [JsonPropertyName("isCorrect")]
    public required bool IsCorrect { get; set; }
} 