using Madarik.Madarik.Data.Roadmap;
using Madarik.Madarik.Data.Topic;
using Microsoft.AspNetCore.Mvc;
using Marten;
using System.ClientModel;

namespace Madarik.Madarik.SubmitTopicQuiz;

internal static class SubmitTopicQuizEndpoint
{
    internal static void MapSubmitTopicQuiz(this IEndpointRouteBuilder app) =>
        app.MapPost(
                MadarikApiPaths.SubmitTopicQuiz,
                async (
                    [FromRoute] Guid roadmapId,
                    [FromRoute] Guid topicId,
                    [FromBody] TopicQuizAnswerSubmission submission,
                    IQuerySession querySession,
                    IDocumentSession documentSession,
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

                    var question = topic.Quiz.Questions.FirstOrDefault(q => q.Id == submission.QuestionId);
                    if (question == null)
                    {
                        return Results.BadRequest("Invalid question ID");
                    }

                    var selectedAnswer = question.PossibleAnswers.FirstOrDefault(a => a.Id == submission.AnswerId);
                    if (selectedAnswer == null)
                    {
                        return Results.BadRequest("Invalid answer ID");
                    }

                    // Check if this question was already answered
                    var existingAnswer = topic.QuizAnswers.Answers.FirstOrDefault(a => a.QuestionId == submission.QuestionId);
                    if (existingAnswer != null)
                    {
                        // Update existing answer
                        existingAnswer.AnswerId = submission.AnswerId;
                        existingAnswer.IsCorrect = selectedAnswer.IsCorrect;
                        existingAnswer.SubmittedAt = DateTimeOffset.UtcNow;
                    }
                    else
                    {
                        // Add new answer
                        topic.QuizAnswers.Answers.Add(new TopicQuizAnswer
                        {
                            QuestionId = submission.QuestionId,
                            AnswerId = submission.AnswerId,
                            IsCorrect = selectedAnswer.IsCorrect,
                            SubmittedAt = DateTimeOffset.UtcNow
                        });
                    }

                    documentSession.Update(roadmap);
                    await documentSession.SaveChangesAsync(cancellationToken);

                    var response = new TopicQuizAnswerResponse
                    {
                        IsCorrect = selectedAnswer.IsCorrect,
                        Explanation = selectedAnswer.IsCorrect ? null : question.Explanation
                    };

                    return Results.Ok(response);
                })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Submit an answer for a topic quiz question",
                Description = "This endpoint allows submitting an answer for a single question in a topic quiz, saves the answer, and provides immediate feedback"
            })
            .AllowAnonymous()
            .Produces<TopicQuizAnswerResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
} 