using System.Text.Json.Serialization;
using Cassandra.DataStax.Graph;

namespace Madarik.Madarik.Data.Roadmap;

internal sealed class Roadmap
{
    [JsonPropertyName("nodes")]
    public required List<RoadmapNode> Nodes { get; set; } 

    [JsonPropertyName("edges")]
    public required List<RoadmapEdge> Edges { get; set; }
}