using System.Text.Json;
using Madarik.Madarik.Data.Roadmap;
using Madarik.Madarik.Data.Topic;
using Microsoft.AspNetCore.Mvc;
using OpenAI;
using OpenAI.Chat;
using Marten;
using System.ClientModel;
using System.Text.Json.Serialization;

namespace Madarik.Madarik.GetQuiz;

internal static class GetQuizEndpoint
{
    private const string GroqEndpoint = "https://api.groq.com/openai/v1";
    
    private static readonly string SystemPrompt = """
You are a specialized AI assistant that creates educational quizzes. Follow these guidelines:

1. Generate quiz questions based on the chapter content and topic
2. Each question should:
    - Be clear and unambiguous
    - Test understanding of key concepts
    - Have 2-4 possible answers
    - Include only one correct answer
    - Provide a detailed explanation of why the correct answer is right

3. Ensure questions are:
    - Directly relevant to the chapter content
    - Progressive in difficulty
    - Testing different aspects of the topic
    - Clear and well-formulated

4. Output must be a valid JSON array of questions, following this format:
[
    {
      "question": "Question text",
      "possibleAnswers": [
        {
          "answer": "Answer text",
          "isCorrect": true/false
        }
      ],
      "explanation": "Detailed explanation of the correct answer"
    }
]

5. Include only the JSON output, no additional text
""";
    
    internal static void MapGetQuiz(this IEndpointRouteBuilder app) =>
        app.MapGet(
                MadarikApiPaths.GetQuiz,
                async (
                    [FromRoute] Guid roadmapId,
                    [FromRoute] Guid topicId,
                    [FromRoute] Guid chapterId,
                    IQuerySession querySession,
                    IDocumentSession documentSession,
                    CancellationToken cancellationToken) =>
                {
                    var roadmap = await querySession.LoadAsync<Roadmap>(roadmapId, cancellationToken);
                    if (roadmap == null)
                    {
                        return Results.NotFound();
                    }

                    var topic = roadmap.Topics.FirstOrDefault(t => t.Id == topicId);
                    if (topic == null)
                    {
                        return Results.NotFound();
                    }

                    var chapter = topic.Chapters.FirstOrDefault(c => c.Id == chapterId);
                    if (chapter == null)
                    {
                        return Results.NotFound();
                    }

                    if (chapter.Quiz != null)
                    {
                        return Results.Ok(QuizResponseDto.ToQuizResponseDto(chapter.Quiz));
                    }

                    ChatClient client = new(
                        model: "gemma2-9b-it",
                        new ApiKeyCredential("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"),
                        new OpenAIClientOptions
                        {
                            Endpoint = new Uri(GroqEndpoint)
                        });

                    var messages = new List<ChatMessage>
                    {
                        new SystemChatMessage(SystemPrompt),
                        new UserChatMessage($"""
Generate a quiz for the chapter "{chapter.Name}" which is part of the topic "{topic.Name}" in the "{roadmap.Name}" learning roadmap.
The quiz should test understanding of the key concepts covered in this chapter:

Chapter Description:
{chapter.Description}

Generate questions that:
- Test understanding of the core concepts
- Vary in difficulty
- Cover different aspects of the chapter content
- Have clear, unambiguous answers
Output as JSON array only.
""")
                    };

                    ChatCompletion completion = await client.CompleteChatAsync(messages, new ChatCompletionOptions(), cancellationToken);
                    var aiResponse = completion.Content[0].Text;

                    int begin = aiResponse.IndexOf('[');
                    int length = aiResponse.LastIndexOf(']') - begin + 1;

                    var questions = JsonSerializer.Deserialize<List<QuizQuestion>>(aiResponse.Substring(
                        begin, length)) ?? throw new BadRequestException("Unable to generate quiz questions, please try again");

                    var quiz = new Quiz { Questions = questions };
                    chapter.Quiz = quiz;
                    
                    documentSession.Update(roadmap);
                    await documentSession.SaveChangesAsync(cancellationToken);

                    return Results.Ok(QuizResponseDto.ToQuizResponseDto(quiz));
                })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Returns a quiz for a specific chapter within a topic and roadmap",
                Description = "This endpoint generates an educational quiz using AI, based on the chapter content and context within the topic and roadmap"
            })
            .AllowAnonymous()
            .Produces<QuizResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

    
}

public class QuizResponseDto
{
    [JsonPropertyName("questions")]
    public required List<QuizQuestionResponseDto> Questions { get; set; }
    public static QuizResponseDto ToQuizResponseDto(Quiz quiz)
    {
        return new QuizResponseDto
        {
            Questions = quiz.Questions.Select(q => new QuizQuestionResponseDto
            {
                Id = q.Id,
                Question = q.Question,
                PossibleAnswers = q.PossibleAnswers.Select(a => new PossibleAnswerResponseDto
                {
                    Id = a.Id,
                    Answer = a.Answer
                }).ToList()
            }).ToList()
        };
    }
}

public class QuizQuestionResponseDto
{
    [JsonPropertyName("id")]
    public required Guid Id { get; set; }

    [JsonPropertyName("question")]
    public required string Question { get; set; }

    [JsonPropertyName("possibleAnswers")]
    public required List<PossibleAnswerResponseDto> PossibleAnswers { get; set; }
}

public class PossibleAnswerResponseDto
{
    [JsonPropertyName("id")]
    public required Guid Id { get; set; }

    [JsonPropertyName("answer")]
    public required string Answer { get; set; }
} 