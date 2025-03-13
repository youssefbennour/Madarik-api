using Newtonsoft.Json.Serialization;

namespace Madarik.Common.Serializers.ContractResolvers;

public class SnakeCaseContractResolver : DefaultContractResolver
{
    public SnakeCaseContractResolver()
        => NamingStrategy = new SnakeCaseNamingStrategy();
}