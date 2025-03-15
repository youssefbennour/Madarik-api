using Marten;

namespace Madarik.Madarik.Data.Database;

public static class DatabaseModule
{
    private const string ConnectionStringName = "Madarik";

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName) 
                               ?? throw new InternalServerException();
        
        services.AddDbContext<SalamHackPersistence>(options => options.UseNpgsql(connectionString));

        services.AddMarten(options =>
        {
            options.Connection(connectionString);
        }).UseLightweightSessions();

        return services;
    }

    public static IApplicationBuilder UseDatabase(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseAutomaticMigrations();

        return applicationBuilder;
    }
}