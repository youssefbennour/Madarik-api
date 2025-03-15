using Madarik.Madarik.Data.Database;

namespace Madarik.Madarik;

public static class MadarikModule
{
    public static IServiceCollection AddContracts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddHttpClient();
        return services;
    }

    public static IApplicationBuilder UseContracts(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseDatabase();

        return applicationBuilder;
    }
}
