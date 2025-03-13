using Madarik.Madarik.GenerateRoadmap;

namespace Madarik.Madarik;

internal static class MadarikEndpoints
{
    internal static void MapContracts(this IEndpointRouteBuilder app)
    {
        app.MapGenerateRoadmap();
    }
}
