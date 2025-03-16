using Madarik.Madarik.Data.Roadmap;
using Madarik.Madarik.Data.Topic;
using Microsoft.AspNetCore.Mvc;
using Marten;
using System.ClientModel;

namespace Madarik.Madarik.SubmitQuiz;

internal static class SubmitQuizEndpoint
{
    internal static void MapSubmitQuiz(this IEndpointRouteBuilder app) =>
        app.MapPost(
                MadarikApiPaths.SubmitQuiz,
                async (
                    [FromRoute] Guid roadmapId,
                    [FromRoute] Guid topicId,
                    [FromRoute] Guid chapterId,
                    [FromBody] List<QuizAnswerSubmission> answers,
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

                    var chapter = topic.Chapters.FirstOrDefault(c => c.Id == chapterId);
                    if (chapter == null)
                    {
                        return Results.NotFound();
                    }

                    if (chapter.Quiz == null)
                    {
                        return Results.NotFound("No quiz found for this chapter");
                    }

                    var quiz = chapter.Quiz;
                    var submission = new List<QuizAnswerResult>();
                    var validAnswers = 0;

                    // Process each question in the quiz
                    foreach (var question in quiz.Questions)
                    {
                        var answer = answers.FirstOrDefault(a => a.QuestionId == question.Id);
                        if (answer == null)
                        {
                            // Question was not answered
                            submission.Add(new QuizAnswerResult
                            {
                                Question = question.Question,
                                YourAnswer = "No answer provided",
                                IsCorrect = false,
                                CorrectAnswer = question.PossibleAnswers.First(a => a.IsCorrect).Answer,
                                Explanation = question.Explanation
                            });
                            continue;
                        }

                        var selectedAnswer = question.PossibleAnswers.FirstOrDefault(a => a.Id == answer.AnswerId);
                        if (selectedAnswer == null)
                        {
                            // Invalid answer ID provided
                            submission.Add(new QuizAnswerResult
                            {
                                Question = question.Question,
                                YourAnswer = "Invalid answer",
                                IsCorrect = false,
                                CorrectAnswer = question.PossibleAnswers.First(a => a.IsCorrect).Answer,
                                Explanation = question.Explanation
                            });
                            continue;
                        }

                        if (selectedAnswer.IsCorrect)
                        {
                            validAnswers++;
                            submission.Add(new QuizAnswerResult
                            {
                                Question = question.Question,
                                YourAnswer = selectedAnswer.Answer,
                                IsCorrect = true
                            });
                        }
                        else
                        {
                            var correctAnswer = question.PossibleAnswers.First(a => a.IsCorrect);
                            submission.Add(new QuizAnswerResult
                            {
                                Question = question.Question,
                                YourAnswer = selectedAnswer.Answer,
                                IsCorrect = false,
                                CorrectAnswer = correctAnswer.Answer,
                                Explanation = question.Explanation
                            });
                        }
                    }

                    var response = new QuizSubmissionResponse
                    {
                        NumberOfQuestions = quiz.Questions.Count,
                        NumberOfValidAnswers = validAnswers,
                        Submission = submission
                    };

                    return Results.Ok(response);
                })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Submit answers for a quiz",
                Description = "This endpoint allows users to submit their answers for a quiz and receive feedback on their performance"
            })
            .AllowAnonymous()
            .Produces<QuizSubmissionResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
} 