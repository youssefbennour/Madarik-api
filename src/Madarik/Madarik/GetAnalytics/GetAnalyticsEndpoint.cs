using Madarik.Madarik.Data.Roadmap;
using Madarik.Madarik.GetTopicQuizResult;
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
                    
                    if (topicState.RoadmapId is null || topicState.Id is null)
                    {
                        return Results.NotFound();
                    }
                    
                    var roadmap = await querySession.LoadAsync<Roadmap>(topicState.RoadmapId, cancellationToken);
                    if (roadmap is null)
                    {
                        return Results.NotFound();
                    }
                    
                    var topic = roadmap.Topics.FirstOrDefault(m => m.Id == topicState.Id);
                    if (topic is null)
                    {
                        return Results.NotFound();
                    }
                    
                    return Results.Ok(new
                    {
                        Id = topic.Id,
                        Name = topic.Name,
                        Description = roadmap.Description,
                        Progress = topic.Progress,
                        Streak = 1,
                        CompletedModules = roadmap.Topics.Count(m => m.IsCompleted || Math.Abs(m.Progress - 100) < 0.1),
                        QuizesTaken = roadmap.Topics.SelectMany(m => m.Chapters).Count(m => m.IsCompleted) +
                                      roadmap.Topics.Count(m => m.IsCompleted)
                        
                    });
                })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Continue where you left off",
                Description = "Gives the last accessed chapter by the user"
            })
            .AllowAnonymous()
            .Produces<TopicQuizResultResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
} 