using Madarik.Madarik.Data.Roadmap;
using Microsoft.AspNetCore.Mvc;
using Marten;

namespace Madarik.Madarik.GetRoadmaps;

internal static class GetRoadmapsEndpoint
{
    internal static void MapGetRoadmaps(this IEndpointRouteBuilder app) =>
        app.MapGet(
                MadarikApiPaths.GetRoadmaps,
                async (
                    IQuerySession querySession,
                    CancellationToken cancellationToken) =>
                {
                    var roadmaps = await Marten.QueryableExtensions.ToListAsync(
                        querySession.Query<Roadmap>(),
                        cancellationToken);

                    var response = roadmaps.Select(roadmap => new RoadmapListItemResponse
                    {
                        Id = roadmap.Id,
                        Name = roadmap.Name,
                        EstimatedTime = roadmap.EstimatedTime,
                        Difficutly = roadmap.Difficulty,
                        Description = roadmap.Description,
                        NumberOfTopics = roadmap.Topics.Count
                    });

                    return Results.Ok(response);
                })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Returns a list of all roadmaps with minimal information",
                Description = "This endpoint retrieves basic information about all roadmaps, including their ID, name, description, and number of topics"
            })
            .AllowAnonymous()
            .Produces<IEnumerable<RoadmapListItemResponse>>(StatusCodes.Status200OK);
}

public class RoadmapListItemResponse
{
    public Guid Id { get; set; }
    public string Difficutly { get; set; } = string.Empty;
    public string EstimatedTime { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int NumberOfTopics { get; set; }
} 