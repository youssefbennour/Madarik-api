using System.Text.Json.Serialization;

namespace Madarik.Madarik.Data.Topic;

public class Quiz
{
    [JsonPropertyName("questions")]
    public required List<QuizQuestion> Questions { get; set; }
}

public class QuizQuestion
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonPropertyName("question")]
    public required string Question { get; set; }

    [JsonPropertyName("possibleAnswers")]
    public required List<PossibleAnswer> PossibleAnswers { get; set; }

    [JsonPropertyName("explanation")]
    public required string Explanation { get; set; }
}

public class PossibleAnswer
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonPropertyName("answer")]
    public required string Answer { get; set; }

    [JsonPropertyName("isCorrect")]
    public required bool IsCorrect { get; set; }
}
