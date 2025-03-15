using System.Text.Json.Serialization;

namespace Madarik.Madarik.Data.Roadmap;

public sealed class FlowChart
{
    [JsonPropertyName("nodes")]
    public required List<FlowChartNode> Nodes { get; set; } 

    [JsonPropertyName("edges")]
    public required List<FlowChartEdge> Edges { get; set; }
}