namespace Madarik.Madarik;

internal static class MadarikApiPaths
{
    private const string ContractsRootApi = $"{ApiPaths.Root}";

    internal const string GenerateRoadmap = $"{ContractsRootApi}/roadmaps";
    internal const string GetTopic = $"{ContractsRootApi}/roadmaps/{{roadmapId}}/topics/{{id}}";
}
