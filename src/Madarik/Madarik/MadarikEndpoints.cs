using Madarik.Madarik.GenerateRoadmap;
using Madarik.Madarik.GetRoadmapById;
using Madarik.Madarik.GetTopic;
using Madarik.Madarik.GetQuiz;
using Madarik.Madarik.SubmitQuiz;

namespace Madarik.Madarik;

public static class MadarikEndpoints
{
    public static void MapContracts(this IEndpointRouteBuilder app)
    {
        var roadmaps = app.MapGroup(string.Empty)
            .WithTags("Roadmaps");
        var topics = app.MapGroup(string.Empty)
            .WithTags("Topics");
        var quizzes = app.MapGroup(string.Empty)
            .WithTags("Quizzes");
         
        roadmaps.MapGenerateRoadmap();
        roadmaps.MapGetRoadmapById();
        
        topics.MapGetTopic();
        
        quizzes.MapGetQuiz();
        quizzes.MapSubmitQuiz();
    }
}
