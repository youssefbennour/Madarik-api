using System.ClientModel;
using System.Text.Json;
using Cassandra;
using Madarik.Madarik.Data.Database;
using Madarik.Madarik.Data.Roadmap;
using Marten;
using OpenAI;
using OpenAI.Chat;

namespace Madarik.Madarik.GenerateRoadmap;

public static class GenerateRoadmapEndpoint
{
    private const string GroqEndpoint = "https://api.groq.com/openai/v1";
    
    private static readonly string SystemPrompt = """
        You are a specialized AI assistant that creates learning roadmaps in ReactFlow JSON format. Follow these guidelines:

        1. Generate a structured learning roadmap based on the user's request
        2. Output must be valid JSON that follows ReactFlow format with these requirements:
           - Must include a "name" property at the root level with a descriptive title for the roadmap
           - Must include a "description" property at the root level with a brief description for the roadmap
           - Each node must have: id, type, position (x, y), data (label), style
           - Each edge must have: id, source, target, type

        3. Node types and positioning:
           - "mainTopic" for core concepts (positioned vertically in the center)
             * Must include style: {
                 backgroundColor: "#F7FBFF",
                 border: "1px solid #3887D9",
                 color: "#3887D9"
               }
             * Can have multiple subTopic nodes as children
           - "subTopic" for related concepts:
             * ALL subtopics of a mainTopic MUST be positioned on the SAME side (either all left or all right)
             * Alternate the side between consecutive mainTopics (if first mainTopic has subtopics on right, next mainTopic's subtopics go on left)
             * When a mainTopic has multiple subtopics on the same side:
                - Stack them vertically with 50px spacing
                - Maintain consistent 200px horizontal spacing from mainTopic
           - Maintain 150px vertical spacing between consecutive mainTopics

        4. Edge types and animations:
           - For mainTopic to subTopic connections:
              * type: "smoothstep"
              * animated: true
              * style: { stroke: "#6366f1" }
           - For prerequisite connections (between main topics):
              * type: "straight"
              * animated: false

        5. Position nodes in a logical flow:
           - Main topics flow from top to bottom in the center (x: 500)
           - All subtopics of a mainTopic go on the same side:
             * Right side position: x = 700
             * Left side position: x = 300
           - Alternate sides between consecutive mainTopics to prevent overlap

        6. Include only the JSON output, no additional text

        Example format:
        {
          "name": "State Management in React",
          "description": "Master modern state management techniques in React applications, from local state to global solutions.",
          "nodes": [
            {
              "id": "1",
              "type": "mainTopic",
              "position": { "x": 500, "y": 100 },
              "data": {
                "label": "Main Topic 1"
              },
              "style": {
                "backgroundColor": "#F7FBFF",
                "border": "1px solid #3887D9",
                "color": "#3887D9"
              }
            },
            {
              "id": "2",
              "type": "subTopic",
              "position": { "x": 700, "y": 75 },
              "data": {
                "label": "Subtopic 1.1"
              }
            },
            {
              "id": "3",
              "type": "subTopic",
              "position": { "x": 700, "y": 125 },
              "data": {
                "label": "Subtopic 1.2"
              }
            },
            {
              "id": "4",
              "type": "mainTopic",
              "position": { "x": 500, "y": 250 },
              "data": {
                "label": "Main Topic 2"
              },
              "style": {
                "backgroundColor": "#F7FBFF",
                "border": "1px solid #3887D9",
                "color": "#3887D9"
              }
            },
            {
              "id": "5",
              "type": "subTopic",
              "position": { "x": 300, "y": 225 },
              "data": {
                "label": "Subtopic 2.1"
              }
            },
            {
              "id": "6",
              "type": "subTopic",
              "position": { "x": 300, "y": 275 },
              "data": {
                "label": "Subtopic 2.2"
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

    public static void MapGenerateRoadmap(this IEndpointRouteBuilder app) =>
        app.MapPost(
                MadarikApiPaths.GenerateRoadmap,
                async (
                  ChatRequest request,
                  IDocumentSession session, 
                  CancellationToken cancellationtoken) =>
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

                    var response = JsonSerializer.Deserialize<AiResponse>(aiResponse.Substring(
                      begin, length)) ?? throw new BadRequestException("Unable to create roadmap, please try again with a different prompt");

                    var flowChart = AiResponse.ToFlowChart(response);
                    var roadmap = new Roadmap(response.Name, response.Description, flowChart);
                    session.Store(roadmap);
                    await session.SaveChangesAsync(cancellationtoken);
                    
                    return Results.Ok(roadmap);
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