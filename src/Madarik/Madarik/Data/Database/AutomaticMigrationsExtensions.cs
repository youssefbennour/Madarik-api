namespace Madarik.Madarik.Data.Database;

public static class AutomaticMigrationsExtensions
{
    public static IApplicationBuilder UseAutomaticMigrations(this IApplicationBuilder applicationBuilder)
    {
        using var scope = applicationBuilder.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SalamHackPersistence>();
        context.Database.Migrate();

        return applicationBuilder;
    }
}