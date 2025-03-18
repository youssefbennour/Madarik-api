namespace Madarik.Madarik.Tracking;


internal sealed class TrackingGrain(
    [PersistentState("topicState", "VolumesStorage")] 
    IPersistentState<TopicState> topicState) : Grain, ITrackingGrain
{
    private IPersistentState<TopicState> topicState = topicState;
    
    public async Task UpdateLatestTopicAsync(Guid roadmapId, Guid id)
    {
        topicState.State.Id = id;
        topicState.State.RoadmapId = roadmapId; 
        await topicState.WriteStateAsync();
    }


    public Task<TopicState> GetLatestTopicAsync()
    {
        return Task.FromResult(topicState.State);
    }
}

[GenerateSerializer]
public class TopicState
{
    [Id(0)]
    public Guid? Id { get; set; }
    [Id(1)]
    public Guid? RoadmapId { get; set; }
}