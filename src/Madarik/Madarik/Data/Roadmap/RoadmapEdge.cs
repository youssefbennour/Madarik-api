using System.Text.Json.Serialization;

namespace Madarik.Madarik.Data.Roadmap;

internal sealed class RoadmapEdge
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

public class EdgeStyle
{
    [JsonPropertyName("stroke")]
    public required string Stroke { get; set; }
}