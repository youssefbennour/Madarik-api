using System.Text.Json.Serialization;
using Madarik.Madarik.Data.Roadmap;

namespace Madarik.Madarik.GenerateRoadmap;

public class AiResponse
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("difficulty")]
    public required string Difficulty { get; set; }

    [JsonPropertyName("estimatedTime")]
    public required string EstimatedTime { get; set; }

    [JsonPropertyName("nodes")]
    public required List<Node> Nodes { get; set; }

    [JsonPropertyName("edges")]
    public required List<Edge> Edges { get; set; }

    public static FlowChart ToFlowChart(AiResponse response)
    {
        // Create a mapping of old string IDs to new GUIDs
        var idMapping = response.Nodes.ToDictionary(
            node => node.Id,
            node => Guid.NewGuid()
        );

        // Convert nodes with new GUIDs
        var nodes = response.Nodes.Select(node => new FlowChartNode
        {
            Id = idMapping[node.Id],
            Type = node.Type,
            Position = new Data.Roadmap.Position 
            { 
                X = node.Position.X, 
                Y = node.Position.Y 
            },
            Data = new Data.Roadmap.NodeData 
            { 
                Label = node.Data.Label 
            },
            Style = node.Style == null ? null : new Data.Roadmap.NodeStyle
            {
                BackgroundColor = node.Style.BackgroundColor,
                Border = node.Style.Border,
                Color = node.Style.Color
            }
        }).ToList();

        // Convert edges with new GUIDs and updated references
        var edges = response.Edges.Select(edge => new FlowChartEdge
        {
            Id = edge.Id,
            Source = idMapping[edge.Source],
            Target = idMapping[edge.Target],
            Type = edge.Type,
            Animated = edge.Animated,
            Style = edge.Style == null ? null : new Data.Roadmap.EdgeStyle
            {
                Stroke = edge.Style.Stroke
            }
        }).ToList();

        return new FlowChart
        {
            Nodes = nodes,
            Edges = edges
        };
    }
}

public class Node
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("position")]
    public required Position Position { get; set; }

    [JsonPropertyName("data")]
    public required NodeData Data { get; set; }

    [JsonPropertyName("style")]
    public NodeStyle? Style { get; set; }
}

public class Edge
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("source")]
    public required string Source { get; set; }

    [JsonPropertyName("target")]
    public required string Target { get; set; }

    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("animated")]
    public bool Animated { get; set; }

    [JsonPropertyName("style")]
    public EdgeStyle? Style { get; set; }
}

public class Position
{
    [JsonPropertyName("x")]
    public required int X { get; set; }

    [JsonPropertyName("y")]
    public required int Y { get; set; }
}

public class NodeData
{
    [JsonPropertyName("label")]
    public required string Label { get; set; }
}

public class NodeStyle
{
    [JsonPropertyName("backgroundColor")]
    public string? BackgroundColor { get; set; }

    [JsonPropertyName("border")]
    public string? Border { get; set; }

    [JsonPropertyName("color")]
    public string? Color { get; set; }
}

public class EdgeStyle
{
    [JsonPropertyName("stroke")]
    public string? Stroke { get; set; }
}
