namespace Madarik.Madarik;

public static class MadarikApiPaths
{
    private const string ContractsRootApi = $"{ApiPaths.Root}";

    public const string GenerateRoadmap = $"{ContractsRootApi}/roadmaps";
    public const string GetRoadmap = $"{ContractsRootApi}/roadmaps/{{id}}";
    public const string GetTopic = $"{ContractsRootApi}/roadmaps/{{roadmapId}}/topics/{{id}}";
    public const string GetQuiz = $"{ContractsRootApi}/roadmaps/{{roadmapId}}/topics/{{topicId}}/chapters/{{chapterId}}/quiz";
    public const string SubmitQuiz = $"{ContractsRootApi}/roadmaps/{{roadmapId}}/topics/{{topicId}}/chapters/{{chapterId}}/quiz/submit";
}
