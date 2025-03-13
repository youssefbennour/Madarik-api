using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Madarik.Common.Telemetry.HealthChecks;
using Madarik.Common.Telemetry.OpenTelemetry;

namespace Madarik.Common.Telemetry {
    public static class TelemetryModule {
        public static IServiceCollection AddTelemetry(this WebApplicationBuilder builder) {
            builder.Host.UseDefaultServiceProvider(options => 
                options.ValidateOnBuild = true);
            builder.AddOpenTelemetryModule();
            builder.Services.AddHealthCheckModule();

            return builder.Services;
        }

        public static IApplicationBuilder UseTelemetry(this WebApplication app) {
            app.UseOpenTelemetry();
            app.UseHealthCheckModule();
            
            return app;
        }
    }
}
