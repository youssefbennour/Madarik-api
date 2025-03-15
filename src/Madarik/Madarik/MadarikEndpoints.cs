using Madarik.Madarik.GenerateRoadmap;
using Madarik.Madarik.GetTopic;

namespace Madarik.Madarik;

internal static class MadarikEndpoints
{
    internal static void MapContracts(this IEndpointRouteBuilder app)
    {
        app.MapGenerateRoadmap();
        app.MapGetTopic();
    }
}
