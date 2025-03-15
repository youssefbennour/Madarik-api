using System.Text.Json.Serialization;

namespace Madarik.Madarik.Data.Roadmap;

public sealed class FlowChartNode
{
    [JsonPropertyName("id")]
    public required Guid Id { get; set; }

    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("position")]
    public required Position Position { get; set; }

    [JsonPropertyName("data")]
    public required NodeData Data { get; set; }

    [JsonPropertyName("style")]
    public NodeStyle? Style { get; set; }
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
