using Madarik.Madarik.Data.Database;
using Madarik.Madarik.Data.Roadmap;
using Marten;

namespace Madarik.Madarik.GetRoadmapById;

public static class GetRoadmapByIdEndpoint
{

    public static void MapGetRoadmapById(this IEndpointRouteBuilder app) =>
        app.MapGet(
                MadarikApiPaths.GetRoadmap,
                async (
                    [FromRoute] Guid id,
                    IQuerySession querySession,
                    CancellationToken cancellationToken) =>
                {
                    var roadmap = await querySession.LoadAsync<Roadmap>(id, cancellationToken);
                    
                    if (roadmap == null)
                    {
                        return Results.NotFound();
                    }
                    
                    return Results.Ok(roadmap);
                })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Returns a learning roadmap",
                Description = "This endpoint generates structured learning roadmap by Id"
            })
            .AllowAnonymous()
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
}