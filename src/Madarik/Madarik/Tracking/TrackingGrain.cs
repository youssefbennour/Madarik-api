using Marten;

namespace Madarik.Madarik.Tracking;


internal sealed class TrackingGrain(
    IDocumentSession documentSession,
    IQuerySession querySession) : Grain, ITrackingGrain
{
    private TopicState topicState = new();

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var topic = await querySession.LoadAsync<TopicState>(Guid.Empty, cancellationToken);
        if (topic != null)
        {
            topicState = topic;
        } 
        
        await base.OnActivateAsync(cancellationToken);
    }

    public async Task UpdateLatestTopicAsync(Guid roadmapId, Guid id)
    {
        topicState.TopicId = id;
        topicState.RoadmapId = roadmapId; 
        documentSession.Store(topicState);
        await documentSession.SaveChangesAsync();
    }


    public Task<TopicState> GetLatestTopicAsync()
    {
        return Task.FromResult(topicState);
    }
}

[GenerateSerializer]
public class TopicState
{
    [Id(0)]
    public Guid Id { get; } = Guid.Empty;
    [Id(1)]
    public Guid? TopicId { get; set; }
    [Id(2)]
    public Guid? RoadmapId { get; set; }
}