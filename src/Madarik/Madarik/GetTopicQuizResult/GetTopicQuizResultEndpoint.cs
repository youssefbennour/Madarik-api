using Madarik.Madarik.Data.Roadmap;
using Microsoft.AspNetCore.Mvc;
using Marten;

namespace Madarik.Madarik.GetTopicQuizResult;

internal static class GetTopicQuizResultEndpoint
{
    internal static void MapGetTopicQuizResult(this IEndpointRouteBuilder app) =>
        app.MapGet(
                MadarikApiPaths.GetTopicQuizResult,
                async (
                    [FromRoute] Guid roadmapId,
                    [FromRoute] Guid topicId,
                    IQuerySession querySession,
                    CancellationToken cancellationToken) =>
                {
                    var roadmap = await querySession.LoadAsync<Roadmap>(roadmapId, cancellationToken);
                    if (roadmap == null)
                    {
                        return Results.NotFound();
                    }

                    var topic = roadmap.Topics.FirstOrDefault(t => t.Id == topicId);
                    if (topic == null)
                    {
                        return Results.NotFound();
                    }

                    if (topic.Quiz == null)
                    {
                        return Results.NotFound("No quiz found for this topic");
                    }

                    var totalQuestions = topic.Quiz.Questions.Count;
                    var correctAnswers = topic.QuizAnswers.Answers.Count(a => a.IsCorrect);
                    
                    // Calculate score as percentage (0 to 100)
                    var score = totalQuestions > 0 
                        ? (double)correctAnswers / totalQuestions * 100 
                        : 0;

                    var response = new TopicQuizResultResponse
                    {
                        NumberOfQuestions = totalQuestions,
                        NumberOfCorrectAnswers = correctAnswers,
                        Score = Math.Round(score, 2) // Round to 2 decimal places
                    };

                    return Results.Ok(response);
                })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get the results of a topic quiz",
                Description = "This endpoint returns the quiz results including total questions, correct answers, and overall score"
            })
            .AllowAnonymous()
            .Produces<TopicQuizResultResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
} 