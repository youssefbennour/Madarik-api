using JasperFx.Core;
using Madarik.Madarik.Data.Roadmap;
using Madarik.Madarik.Tracking;
using Marten;

namespace Madarik.Madarik.GetAnalytics;

internal static class GetAnalyticsEndpoint
{
    internal static void MapGetAnalyticsEndpoint(this IEndpointRouteBuilder app) =>
        app.MapGet(
                MadarikApiPaths.GetAnalytics,
                async (
                    IQuerySession querySession,
                    IGrainFactory grainFactory,
                    CancellationToken cancellationToken) =>
                {
                    var grain = grainFactory.GetGrain<ITrackingGrain>(Guid.Empty);
                    var topicState = await grain.GetLatestTopicAsync();

                    var roadmaps= querySession.Query<Roadmap>().ToList();
                    
                    
                    var response = new RoadmapProgressResponse
                    {
                        Streak = 1,
                        CompletedModules = roadmaps.SelectMany(m => m.Topics).Count(m => m.IsCompleted || Math.Abs(m.Progress - 100) < 0.1),
                        QuizesTaken = roadmaps.SelectMany(m => m.Topics).SelectMany(m => m.Chapters).Count(m => m.IsCompleted) +
                                      roadmaps.SelectMany(m => m.Topics).Count(m => m.IsCompleted)
                    };


                    if (topicState.RoadmapId is null || topicState.TopicId is null)
                    {
                        return Results.Ok(response);
                    }
                     
                    var roadmap = roadmaps.FirstOrDefault(m => m.Id == topicState.RoadmapId);
                     
                    var topic = roadmap?.Topics.FirstOrDefault(m => m.Id == topicState.TopicId);

                    if (roadmap is null || topic is null)
                    {
                        return Results.Ok(response);
                    }
                    
                    if (topic.IsCompleted || Math.Abs(topic.Progress - 100) < 0.1)
                    {
                        var tempTopic = roadmap.Topics.FirstOrDefault(m => m is { IsCompleted: false, Progress: < 100 });
                        if (tempTopic is not null)
                        {
                            topic = tempTopic;
                        }
                    }

                    response.Topic ??= new();
                    response.Topic.Id = topic.Id;
                    response.Topic.RoadmapId = roadmap.Id;
                    response.Topic.Name = topic.Name;
                    response.Topic.Description = roadmap.Description ?? string.Empty;
                    response.Topic.Progress = topic.Progress;
                    
                    return Results.Ok(response);
                })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Continue where you left off",
                Description = "Gives the last accessed chapter by the user"
            })
            .AllowAnonymous()
            .Produces(StatusCodes.Status404NotFound);
} 

public class RoadmapProgressResponse
{
    public TopicResponse? Topic { get; set; }
    public int Streak { get; set; }
    public int CompletedModules { get; set; }
    public int QuizesTaken { get; set; }
}

public class TopicResponse
{
    public Guid Id { get; set; }
    public Guid RoadmapId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Progress { get; set; }
}