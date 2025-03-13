using System.ClientModel;
using System.Text.Json;
using Madarik.Madarik.Data.Roadmap;
using OpenAI;
using OpenAI.Chat;

namespace Madarik.Madarik.GenerateRoadmap;

internal static class GenerateRoadmapEndpoint
{
    private const string GroqEndpoint = "https://api.groq.com/openai/v1";
    
    private static readonly string SystemPrompt = """
you are a specialized ai assistant that creates learning roadmaps in reactflow json format. follow these guidelines:

1. generate a structured learning roadmap based on the user's request
2. output must be valid json that follows reactflow format with these requirements:
   - each node must have: id, type, position (x, y), data (label), style
   - each edge must have: id, source, target, type

3. node types and positioning:
   - "maintopic" for core concepts (positioned vertically in the center)
     * must include style: {
         backgroundcolor: "#f7fbff",
         border: "1px solid #3887d9",
         color: "#3887d9"
       }
     * can have multiple subtopic nodes as children
   - "subtopic" for related concepts:
     * all subtopics of a maintopic must be positioned on the same side (either all left or all right)
     * alternate the side between consecutive maintopics (if first maintopic has subtopics on right, next maintopic's subtopics go on left)
     * when a maintopic has multiple subtopics on the same side:
        - stack them vertically with 50px spacing
        - maintain consistent 200px horizontal spacing from maintopic
   - maintain 150px vertical spacing between consecutive maintopics

4. edge types and animations:
   - for maintopic to subtopic connections:
      * type: "smoothstep"
      * animated: true
      * style: { stroke: "#6366f1" }
   - for prerequisite connections (between main topics):
      * type: "straight"
      * animated: false

5. position nodes in a logical flow:
   - main topics flow from top to bottom in the center (x: 500)
   - all subtopics of a maintopic go on the same side:
     * right side position: x = 700
     * left side position: x = 300
   - alternate sides between consecutive maintopics to prevent overlap

6. include only the json output, no additional text

example format:
{
  "nodes": [
    {
      "id": "1",
      "type": "maintopic",
      "position": { "x": 500, "y": 100 },
      "data": {
        "label": "main topic 1"
      },
      "style": {
        "backgroundcolor": "#f7fbff",
        "border": "1px solid #3887d9",
        "color": "#3887d9"
      }
    },
    {
      "id": "2",
      "type": "subtopic",
      "position": { "x": 700, "y": 75 },
      "data": {
        "label": "subtopic 1.1"
      }
    },
    {
      "id": "3",
      "type": "subtopic",
      "position": { "x": 700, "y": 125 },
      "data": {
        "label": "subtopic 1.2"
      }
    },
    {
      "id": "4",
      "type": "maintopic",
      "position": { "x": 500, "y": 250 },
      "data": {
        "label": "main topic 2"
      },
      "style": {
        "backgroundcolor": "#f7fbff",
        "border": "1px solid #3887d9",
        "color": "#3887d9"
      }
    },
    {
      "id": "5",
      "type": "subtopic",
      "position": { "x": 300, "y": 225 },
      "data": {
        "label": "subtopic 2.1"
      }
    },
    {
      "id": "6",
      "type": "subtopic",
      "position": { "x": 300, "y": 275 },
      "data": {
        "label": "subtopic 2.2"
      }
    }
  ],
  "edges": [
    {
      "id": "e1-2",
      "source": "1",
      "target": "2",
      "type": "smoothstep",
      "animated": true,
      "style": { "stroke": "#6366f1" }
    },
    {
      "id": "e1-3",
      "source": "1",
      "target": "3",
      "type": "smoothstep",
      "animated": true,
      "style": { "stroke": "#6366f1" }
    },
    {
      "id": "e4-5",
      "source": "4",
      "target": "5",
      "type": "smoothstep",
      "animated": true,
      "style": { "stroke": "#6366f1" }
    },
    {
      "id": "e4-6",
      "source": "4",
      "target": "6",
      "type": "smoothstep",
      "animated": true,
      "style": { "stroke": "#6366f1" }
    },
    {
      "id": "e1-4",
      "source": "1",
      "target": "4",
      "type": "straight",
      "animated": false
    }
  ]
}
""";

    internal static void MapGenerateRoadmap(this IEndpointRouteBuilder app) =>
        app.MapPost(
                MadarikApiPaths.GenerateRoadmap,
                async (ChatRequest request, CancellationToken cancellationtoken) =>
                {
                    ChatClient client = new(
                        model: "deepseek-r1-distill-llama-70b", 
                        new ApiKeyCredential("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"), 
                        new OpenAIClientOptions
                        {
                            Endpoint = new Uri(GroqEndpoint)
                        });
                
                    var messages = new List<ChatMessage>
                    {
                        new SystemChatMessage(SystemPrompt),
                        new UserChatMessage($"""
                                             Generate a learning roadmap for: {request.Message}
                                             Consider:
                                             - Core topics and their related subtopics
                                             - Logical progression of topics
                                             - Clear relationships between topics
                                             Output as ReactFlow JSON only.
                                             """)
                    };

                    ChatCompletion completion = await client.CompleteChatAsync(messages, new ChatCompletionOptions(), cancellationtoken);
                    var aiResponse = completion.Content[0].Text;
                        
                    int begin = aiResponse.IndexOf('{');
                    int length = aiResponse.LastIndexOf('}') - begin + 1;
                    
                    var response = JsonSerializer.Deserialize<Roadmap>(aiResponse.Substring(
                      begin, length));
                        
                    return Results.Ok(response);
                })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Returns a learning roadmap in ReactFlow JSON format using Groq API",
                Description = "This endpoint generates structured learning roadmaps based on user queries, formatted as ReactFlow-compatible JSON"
            })
            .AllowAnonymous()
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
}
public record ChatRequest(string Message);