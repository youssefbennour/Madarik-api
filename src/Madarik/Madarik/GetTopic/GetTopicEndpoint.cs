using System.Text.Json;
using System.Text.Json.Serialization;
using Madarik.Madarik.Data.Database;
using Madarik.Madarik.Data.Topic;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Web;
using Google.Apis.CustomSearchAPI.v1;
using Google.Apis.Requests;
using Google.Apis.Services;
using Madarik.Madarik.Data.Roadmap;
using Madarik.Madarik.Tracking;
using Marten;
using Marten.Internal.Sessions;

namespace Madarik.Madarik.GetTopic;

internal static class GetTopicEndpoint
{
    private const string GroqEndpoint = "https://api.groq.com/openai/v1";
    private const string GoogleSearchApiKey = "AIzaSyA78dehMs-kZmt-wucZPpCrV3Vzq9bWvxM";
    private const string GoogleSearchEngineId = "929e13411e2bf4dcd";
    
    private static readonly string SystemPrompt = """
You are a specialized AI assistant that creates detailed course chapters. Follow these guidelines:

1. Generate chapters based on the user's topic request
2. Each chapter should include:
    - A clear, concise name that aligns with the main topic and its parent roadmap context
    - A detailed description explaining the chapter's content, importance, and how it fits within the broader roadmap
    - A specific search query that would help find relevant articles for this chapter
   (this query will be used to find documentation and tutorials)

3. Ensure chapters are:
    - Directly relevant to the topic
    - Progressive in difficulty
    - Comprehensive in coverage
    - Aligned with both the specific topic and the broader roadmap context

4. Output must be a valid JSON object with two fields:
   - difficulty: The overall difficulty level of the topic (must be one of: "Beginner", "Intermediate", or "Advanced")
   - chapters: An array of chapters, following this format:
{
  "difficulty": "Intermediate",
  "chapters": [
    {
      "name": "Chapter Name",
      "description": "Detailed chapter description",
      "searchQuery": "specific search query for finding relevant articles"
    }
  ]
}

5. Include only the JSON output, no additional text
""";
    
    internal static void MapGetTopic(this IEndpointRouteBuilder app) =>
        app.MapGet(
                MadarikApiPaths.GetTopic,
                async (
                    [FromRoute] Guid id,
                    [FromRoute] Guid roadmapId,
                    IQuerySession querySession,
                    IDocumentSession documentSession,
                    IGrainFactory grainFactory,
                    CancellationToken cancellationToken) =>
                {
                    
                    var roadmap = await querySession.LoadAsync<Roadmap>(roadmapId, cancellationToken);
                    if (roadmap == null)
                    {
                        return Results.NotFound(); 
                    }

                    if (roadmap.Topics.FirstOrDefault(topic => topic.Id == id) is { } existingTopic)
                    {
                        await grainFactory.GetGrain<ITrackingGrain>(Guid.Empty).UpdateLatestTopicAsync(roadmapId, id);
                        return Results.Ok(existingTopic);
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
 Generate detailed chapters for the topic "{topicName}" which is part of the "{roadmap.Name}" learning roadmap.
 The content should be specifically tailored to fit within the context of {roadmap.Name} while diving deep into {topicName}.
 For each chapter, provide a specific search query that will help find relevant documentation and tutorials.
 Consider:
 - Progressive learning path
 - Clear chapter progression
 - Comprehensive coverage of the topic
 - Overall difficulty level of the topic
 Output as JSON object only.
""")
                    };

                    ChatCompletion completion = await client.CompleteChatAsync(messages, new ChatCompletionOptions(), cancellationToken);
                    var aiResponse = completion.Content[0].Text;

                    int begin = aiResponse.IndexOf('{');
                    int length = aiResponse.LastIndexOf('}') - begin + 1;

                    var response = JsonSerializer.Deserialize<TopicGenerationResponse>(aiResponse.Substring(
                        begin, length)) ?? throw new BadRequestException("Unable to generate topic content, please try again with a different topic");

                    var chapters = new List<Chapter>();
                    var chaptersWithSearchQuery = await FetchArticlesPerChapterAsync(
                        topicName, 
                        response.Chapters);
                    
                    chaptersWithSearchQuery.ForEach(m =>
                    {
                        chapters.Add(new Chapter
                        {
                            Name = m.Name,
                            Description = m.Description,
                            Articles = m.Articles,
                        });
                    });
                    
                    await grainFactory.GetGrain<ITrackingGrain>(Guid.Empty).UpdateLatestTopicAsync(roadmapId, id);
                        
                    var topic = new Topic(id, topicName, chapters)
                    {
                        Difficulty = response.Difficulty
                    };
                    roadmap.Topics.Add(topic);
                    documentSession.Update(roadmap);
                    await documentSession.SaveChangesAsync(cancellationToken);

                    return Results.Ok(topic);
                })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Returns a list of chapters with curated articles for a given topic within a roadmap",
                Description = "This endpoint generates structured learning content with relevant articles using AI and Google Search, contextualized within a specific roadmap"
            })
            .AllowAnonymous()
            .Produces<Topic>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    

    private static async Task<List<ChapterWithSearchQuery>> FetchArticlesPerChapterAsync(string topicName,
        List<ChapterWithSearchQuery> chapters)
    {
        var service = new CustomSearchAPIService(new BaseClientService.Initializer
        {
            ApiKey = GoogleSearchApiKey,
            ApplicationName = "SalamHack"
        });
        
        var request = new BatchRequest(service);
        
        
        foreach (var chapter in chapters)
        {
            var searchRequest = service.Cse.List();
            searchRequest.Q = $"{topicName} {chapter.SearchQuery}";
            searchRequest.Cx = GoogleSearchEngineId; 
            
            request.Queue<Google.Apis.CustomSearchAPI.v1.Data.Search>(
                searchRequest,
                (content, error, i, message) =>
                {
                    if (content?.Items is null)
                    {
                        return;
                    }
                    chapter.Articles.AddRange(content.Items.Select(m => new Article
                    {
                        Name = m.Title,
                        Url = m.Link,
                    }));

                    chapter.Articles = chapter.Articles.Take(2).ToList();
                });
        }
        
        await request.ExecuteAsync();


        return chapters;
    }
}

internal class TopicGenerationResponse
{
    [JsonPropertyName("difficulty")]
    public required string Difficulty { get; set; }

    [JsonPropertyName("chapters")]
    public required List<ChapterWithSearchQuery> Chapters { get; set; }
}

internal class ChapterWithSearchQuery
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("searchQuery")]
    public required string SearchQuery { get; set; }

    [JsonIgnore] public List<Article> Articles { get; set; } = new();
}

internal class GoogleSearchResponse
{
    [JsonPropertyName("items")]
    public List<GoogleSearchResult>? Items { get; set; }
}

internal class GoogleSearchResult
{
    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("link")]
    public required string Link { get; set; }

    [JsonPropertyName("publisher")]
    public string? Publisher { get; set; }
}

public record TopicRequest(string Topic);