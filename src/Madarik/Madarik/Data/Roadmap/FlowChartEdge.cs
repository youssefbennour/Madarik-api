using System.Text.Json.Serialization;

namespace Madarik.Madarik.Data.Roadmap;

internal sealed class FlowChartEdge
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("source")]
    public required Guid Source { get; set; }
    [JsonPropertyName("target")]
    public required Guid Target { get; set; }

    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("animated")]
    public bool Animated { get; set; }

    [JsonPropertyName("style")]
    public EdgeStyle? Style { get; set; }
}

public class EdgeStyle
{
    [JsonPropertyName("stroke")]
    public required string Stroke { get; set; }
}