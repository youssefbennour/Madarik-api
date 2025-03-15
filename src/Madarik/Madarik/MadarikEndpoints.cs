using Madarik.Madarik.GenerateRoadmap;
using Madarik.Madarik.GetRoadmapById;
using Madarik.Madarik.GetTopic;

namespace Madarik.Madarik;

public static class MadarikEndpoints
{
    public static void MapContracts(this IEndpointRouteBuilder app)
    {
        var roadmaps= app.MapGroup(string.Empty)
            .WithTags("Roadmaps");
         var topics = app.MapGroup(string.Empty)
            .WithTags("Topics");
         
        roadmaps.MapGenerateRoadmap();
        roadmaps.MapGetRoadmapById();
        
        topics.MapGetTopic();
    }
}
