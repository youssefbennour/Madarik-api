using Madarik.Madarik.Data.Database;

namespace Madarik.Madarik;

internal static class MadarikModule
{
    internal static IServiceCollection AddContracts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddHttpClient();
        return services;
    }

    internal static IApplicationBuilder UseContracts(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseDatabase();

        return applicationBuilder;
    }
}
