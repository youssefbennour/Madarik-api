using System.Text.Json;
using Madarik.Madarik.Data.Database;
using Madarik.Madarik.Data.Topic;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace Madarik.Madarik.GetTopic;

internal static class GetTopicEndpoint
{
    private const string GroqEndpoint = "https://api.groq.com/openai/v1";
    
    private static readonly string SystemPrompt = """
        You are a specialized AI assistant that creates detailed course chapters with relevant articles. Follow these guidelines:

        1. Generate chapters based on the user's topic request
        2. Each chapter should include:
           - A clear, concise name that aligns with the main topic and its parent roadmap context
           - A detailed description explaining the chapter's content, importance, and how it fits within the broader roadmap
           - A curated list of 1-2 high-quality articles that are directly relevant to the chapter (STRICT MAXIMUM OF 2 ARTICLES)
        3. For each article, provide:
           - Name: Clear and descriptive title
           - Estimated reading time in minutes
           - URL: Link to reputable sources (prefer well-known tech blogs, documentation, or educational platforms)
             * Must be a valid URL
           - Author: The content creator's name when available

        4. Ensure articles are:
           - Limited to maximum 2 per chapter
           - Directly relevant to the chapter's topic
           - From reputable sources
           - Varied in length and depth to accommodate different learning styles
           - Current and up-to-date
           - Aligned with both the specific topic and the broader roadmap context

        5. Output must be a valid JSON array of chapters, following this format:
        [
          {
            "name": "Chapter Name",
            "description": "Detailed chapter description",
            "articles": [
              {
                "name": "Article Title",
                "estimatedTime": "X min",
                "url": "https://example.com/article",
                "author": "Author Name"
              }
            ]
          }
        ]

        6. Include only the JSON output, no additional text
        """;

    internal static void MapGetTopic(this IEndpointRouteBuilder app) =>
        app.MapPost(
                MadarikApiPaths.GetTopic,
                async (
                    [FromRoute] Guid id,
                    [FromRoute] Guid roadmapId,
                    SalamHackPersistence persistence,
                    CancellationToken cancellationToken) =>
                {
                    var roadmap = await persistence.Roadmaps
                        .FirstOrDefaultAsync(roadmap => roadmap.Id == roadmapId, cancellationToken: cancellationToken);
                    if (roadmap == null)
                    {
                       return Results.NotFound(); 
                    }

                    var topicName = roadmap.FlowChart.Nodes
                        .Where(m => m.Id == id)
                        .Select(m => m.Data.Label)
                        .FirstOrDefault();

                    if (topicName == null)
                    {
                        return Results.NotFound();
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
                            Generate detailed chapters with relevant articles for the topic "{topicName}" which is part of the "{roadmap.Name}" learning roadmap.
                            The content should be specifically tailored to fit within the context of {roadmap.Name} while diving deep into {topicName}.
                            Consider:
                            - Progressive learning path
                            - Mix of theoretical and practical articles
                            - Variety of content difficulty levels
                            Output as JSON array only.
                            """)
                    };

                    ChatCompletion completion = await client.CompleteChatAsync(messages, new ChatCompletionOptions(), cancellationToken);
                    var aiResponse = completion.Content[0].Text;

                    int begin = aiResponse.IndexOf('[');
                    int length = aiResponse.LastIndexOf(']') - begin + 1;

                    var chapters = JsonSerializer.Deserialize<List<Chapter>>(aiResponse.Substring(
                        begin, length)) ?? throw new BadRequestException("Unable to generate chapters, please try again with a different topic");

                    var topic = new Topic(topicName, chapters);
                    persistence.Topics
                        .Add(topic);

                    await persistence.SaveChangesAsync(cancellationToken);

                    return Results.Ok(topic);
                })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Returns a list of chapters with curated articles for a given topic within a roadmap",
                Description = "This endpoint generates structured learning content with relevant articles using AI, contextualized within a specific roadmap"
            })
            .AllowAnonymous()
            .Produces<Topic>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
}

public record TopicRequest(string Topic);