using Madarik.Madarik.Data.Roadmap;
using Marten;

namespace Madarik.Madarik.DeleteRoadmap;

public static class DeleteRoadmapEndpoint
{
    internal static void MapDeleteRoadmap(this IEndpointRouteBuilder app) =>
        app.MapDelete(
                MadarikApiPaths.DeleteRoadmap,
                async (
                    [FromRoute] Guid id,
                    IQuerySession querySession,
                    IDocumentSession documentSession,
                    CancellationToken cancellationToken) =>
                {
                    var roadmap = await querySession.LoadAsync<Roadmap>(id, cancellationToken);
                    if (roadmap is null)
                    {
                        return Results.NotFound();
                    }

                    documentSession.Delete(roadmap);
                    await documentSession.SaveChangesAsync(cancellationToken);

                    return Results.Ok();
                })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Deletes roadmap",
                Description = "Deletes roadmap by Id"
            })
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
}