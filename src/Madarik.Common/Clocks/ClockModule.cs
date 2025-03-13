using Microsoft.Extensions.DependencyInjection;

namespace Madarik.Common.Clocks;

public static class ClockModule
{
    public static IServiceCollection AddClock(this IServiceCollection services) =>
        services.AddSingleton(TimeProvider.System);
}
