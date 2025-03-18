using System.Text.Json.Serialization;

namespace Madarik.Madarik.Data.Roadmap;

public sealed class Roadmap
{
#pragma warning disable CS8618, CS9264
    private Roadmap()
#pragma warning restore CS8618, CS9264
    {
        
    }

    public Roadmap(string name, string description, FlowChart flowChart, string difficulty, string estimatedTime)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        FlowChart = flowChart;
        Difficulty = difficulty;
        EstimatedTime = estimatedTime;
    }
    
    public Guid Id { get; init; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Difficulty { get; set; }
    public string EstimatedTime { get; set; }
    [JsonIgnore]
    public List<Topic.Topic> Topics { get; set; } = new();
    public FlowChart FlowChart { get; set; }
}