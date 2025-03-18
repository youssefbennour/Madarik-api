namespace Madarik.Madarik.Tracking;

public interface ITrackingGrain : IGrainWithGuidKey
{
    Task UpdateLatestTopicAsync(Guid roadmapId, Guid id);
    Task<TopicState> GetLatestTopicAsync();
}