using Madarik.Madarik.DeleteRoadmap;
using Madarik.Madarik.GenerateRoadmap;
using Madarik.Madarik.GetAnalytics;
using Madarik.Madarik.GetRoadmapById;
using Madarik.Madarik.GetRoadmaps;
using Madarik.Madarik.GetTopic;
using Madarik.Madarik.GetQuiz;
using Madarik.Madarik.SubmitQuiz;
using Madarik.Madarik.GetTopicQuiz;
using Madarik.Madarik.SubmitTopicQuiz;
using Madarik.Madarik.GetTopicQuizResult;
using Madarik.Madarik.Tracking;

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
        roadmaps.MapGetRoadmaps();
        roadmaps.MapGetRoadmapById();
        roadmaps.MapDeleteRoadmap();
        
        topics.MapGetTopic();
        topics.MapGetAnalyticsEndpoint();
        
        quizzes.MapGetQuiz();
        quizzes.MapGetTopicQuiz();
        quizzes.MapSubmitQuiz();
        quizzes.MapSubmitTopicQuiz();
        quizzes.MapGetTopicQuizResult();
    }
}
