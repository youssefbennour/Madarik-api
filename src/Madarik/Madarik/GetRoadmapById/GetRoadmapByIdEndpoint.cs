using Madarik.Madarik.Data.Database;

namespace Madarik.Madarik.GetRoadmapById;

public static class GetRoadmapByIdEndpoint
{

    public static void MapGetRoadmapById(this IEndpointRouteBuilder app) =>
        app.MapGet(
                MadarikApiPaths.GetRoadmap,
                async (
                    [FromRoute] Guid id,
                  SalamHackPersistence persistence, 
                  CancellationToken cancellationtoken) =>
                {
                    var roadmap = await persistence.Roadmaps
                        .FirstOrDefaultAsync(m => m.Id ==  id, cancellationtoken);
                    
                    if (roadmap == null)
                    {
                        return Results.NotFound();
                    }
                    
                    return Results.Ok(roadmap);
                })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Returns a learning roadmap in ReactFlow JSON format using Groq API",
                Description = "This endpoint generates structured learning roadmaps based on user queries, formatted as ReactFlow-compatible JSON"
            })
            .AllowAnonymous()
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
}