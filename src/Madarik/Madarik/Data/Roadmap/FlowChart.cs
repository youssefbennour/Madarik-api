using System.Text.Json.Serialization;

namespace Madarik.Madarik.Data.Roadmap;

internal sealed class FlowChart
{
    [JsonPropertyName("nodes")]
    public required List<FlowChartNode> Nodes { get; set; } 

    [JsonPropertyName("edges")]
    public required List<FlowChartEdge> Edges { get; set; }
}