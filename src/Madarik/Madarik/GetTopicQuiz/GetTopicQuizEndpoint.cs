using System.Text.Json;
using Madarik.Madarik.Data.Roadmap;
using Madarik.Madarik.Data.Topic;
using Madarik.Madarik.GetQuiz;
using Microsoft.AspNetCore.Mvc;
using OpenAI;
using OpenAI.Chat;
using Marten;
using System.ClientModel;
using System.Text.Json.Serialization;

namespace Madarik.Madarik.GetTopicQuiz;

internal static class GetTopicQuizEndpoint
{
    private const string GroqEndpoint = "https://api.groq.com/openai/v1";
    
    private static readonly string SystemPrompt = """
You are a specialized AI assistant that creates educational quizzes. Follow these guidelines:

1. Generate quiz questions that cover the entire topic and all its chapters
2. Each question should:
    - Be clear and unambiguous
    - Test understanding of key concepts
    - Have 2-4 possible answers
    - Include only one correct answer
    - Provide a detailed explanation of why the correct answer is right

3. Ensure questions are:
    - Cover concepts from all chapters in the topic
    - Progressive in difficulty
    - Testing different aspects across chapters
    - Clear and well-formulated
    - Between 10 and 20 questions in total

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
    
    internal static void MapGetTopicQuiz(this IEndpointRouteBuilder app) =>
        app.MapGet(
                MadarikApiPaths.GetTopicQuiz,
                async (
                    [FromRoute] Guid roadmapId,
                    [FromRoute] Guid topicId,
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

                    if (topic.Quiz != null)
                    {
                        return Results.Ok(QuizResponseDto.ToQuizResponseDto(topic.Quiz));
                    }

                    if (!topic.Chapters.Any())
                    {
                        return Results.BadRequest("Topic has no chapters to generate quiz from");
                    }

                    ChatClient client = new(
                        model: "gemma2-9b-it",
                        new ApiKeyCredential("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"),
                        new OpenAIClientOptions
                        {
                            Endpoint = new Uri(GroqEndpoint)
                        });

                    // Build a comprehensive context of all chapters
                    var chaptersContext = string.Join("\n\n", topic.Chapters.Select(c => 
                        $"Chapter: {c.Name}\nDescription: {c.Description}"));

                    var messages = new List<ChatMessage>
                    {
                        new SystemChatMessage(SystemPrompt),
                        new UserChatMessage($"""
Generate a comprehensive quiz for the topic "{topic.Name}" which is part of the "{roadmap.Name}" learning roadmap.
The quiz should test understanding of key concepts across all chapters in this topic:

Topic Chapters:
{chaptersContext}

Generate questions that:
- Cover concepts from all chapters
- Test understanding of core concepts
- Vary in difficulty
- Have clear, unambiguous answers
- Total between 10 and 20 questions
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
                    topic.Quiz = quiz;
                    
                    documentSession.Update(roadmap);
                    await documentSession.SaveChangesAsync(cancellationToken);

                    return Results.Ok(QuizResponseDto.ToQuizResponseDto(quiz));
                })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Returns a comprehensive quiz for an entire topic",
                Description = "This endpoint generates a quiz that covers all chapters within a topic using AI, with 10-20 questions testing understanding across the entire topic"
            })
            .AllowAnonymous()
            .Produces<QuizResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
} 