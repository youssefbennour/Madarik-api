using System.Text.Json;
using System.Text.Json.Serialization;
using Madarik.Madarik.Data.Database;
using Madarik.Madarik.Data.Topic;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Web;

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

4. Output must be a valid JSON array of chapters, following this format:
[
    {
      "name": "Chapter Name",
      "description": "Detailed chapter description",
      "searchQuery": "specific search query for finding relevant articles"
    }
]

5. Include only the JSON output, no additional text
""";
    
    internal static void MapGetTopic(this IEndpointRouteBuilder app) =>
        app.MapGet(
                MadarikApiPaths.GetTopic,
                async (
                    [FromRoute] Guid id,
                    [FromRoute] Guid roadmapId,
                    SalamHackPersistence persistence,
                    IHttpClientFactory httpClientFactory,
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
                        model: "mixtral-8x7b-32768",
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
                                             Output as JSON array only.
                                             """)
                    };

                    ChatCompletion completion = await client.CompleteChatAsync(messages, new ChatCompletionOptions(), cancellationToken);
                    var aiResponse = completion.Content[0].Text;

                    int begin = aiResponse.IndexOf('[');
                    int length = aiResponse.LastIndexOf(']') - begin + 1;

                    var chaptersWithoutArticles = JsonSerializer.Deserialize<List<ChapterWithSearchQuery>>(aiResponse.Substring(
                        begin, length)) ?? throw new BadRequestException("Unable to generate chapters, please try again with a different topic");

                    using var httpClient = httpClientFactory.CreateClient();
                    
                    var chapters = new List<Chapter>();
                    foreach (var chapterWithoutArticles in chaptersWithoutArticles)
                    {
                        var articles = await FetchArticlesForChapter(
                            $"{topicName} {chapterWithoutArticles.SearchQuery}",
                            httpClient,
                            cancellationToken);

                        chapters.Add(new Chapter
                        {
                            Name = chapterWithoutArticles.Name,
                            Description = chapterWithoutArticles.Description,
                            Articles = articles
                        });
                    }

                    var topic = new Topic(topicName, chapters);
                    persistence.Topics
                        .Add(topic);

                    await persistence.SaveChangesAsync(cancellationToken);

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
    
    private static async Task<List<Article>> FetchArticlesForChapter(string searchQuery, HttpClient httpClient, CancellationToken cancellationToken)
    {
        var articles = new List<Article>();
            
        var tutorialQuery = $"{searchQuery} tutorial guide";
        var encodedTutorialQuery = HttpUtility.UrlEncode(tutorialQuery);
        var tutorialUrl = $"https://www.googleapis.com/customsearch/v1?key={GoogleSearchApiKey}&cx={GoogleSearchEngineId}&q={encodedTutorialQuery}";
            
        var tutorialResponse = await httpClient.GetStringAsync(tutorialUrl, cancellationToken);
        var tutorialResults = JsonSerializer.Deserialize<GoogleSearchResponse>(tutorialResponse);
            
        for(int index = 0; index < tutorialResults?.Items?.Count; index++){
            if(index > 1){
                break;
            }
            var tutorial = tutorialResults.Items[index];
            articles.Add(new Article
            {
                Name = tutorial.Title,
                Url = tutorial.Link,
                Author = tutorial.Publisher
            });
    
        }
    
        return articles;
    }
}

internal class ChapterWithSearchQuery
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("searchQuery")]
    public required string SearchQuery { get; set; }
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