namespace Madarik.Madarik.Data.Roadmap;

internal sealed class Roadmap
{
#pragma warning disable CS8618, CS9264
    private Roadmap()
#pragma warning restore CS8618, CS9264
    {
        
    }

    public Roadmap(string name, string description, FlowChart flowChart)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        FlowChart = flowChart;
    }
    
    public Guid Id { get; init; }
    public string Name { get; set; }
    public string Description { get; set; }
    public FlowChart FlowChart { get; set; }
}