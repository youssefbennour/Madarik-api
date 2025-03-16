using System.Text.Json.Serialization;

namespace Madarik.Madarik.SubmitQuiz;

public class QuizAnswerSubmission
{
    [JsonPropertyName("questionId")]
    public required Guid QuestionId { get; set; }

    [JsonPropertyName("answerId")]
    public required Guid AnswerId { get; set; }
}

public class QuizSubmissionResponse
{
    [JsonPropertyName("numberOfQuestions")]
    public required int NumberOfQuestions { get; set; }

    [JsonPropertyName("numberOfValidAnswers")]
    public required int NumberOfValidAnswers { get; set; }

    [JsonPropertyName("submission")]
    public required List<QuizAnswerResult> Submission { get; set; }
}

public class QuizAnswerResult
{
    [JsonPropertyName("question")]
    public required string Question { get; set; }

    [JsonPropertyName("yourAnswer")]
    public required string YourAnswer { get; set; }

    [JsonPropertyName("isCorrect")]
    public required bool IsCorrect { get; set; }

    [JsonPropertyName("correctAnswer")]
    public string? CorrectAnswer { get; set; }

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }
} 