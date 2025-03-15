
using Microsoft.Extensions.DependencyInjection;

namespace Madarik.Common.ApiConfiguration.Cors;

public static class CorsExtensions
{
   public static IServiceCollection AddCorsPolicies(this IServiceCollection services)
   {
      return services.AddCors(options =>
      {
         options.AddDefaultPolicy(
            builder =>
            {
               builder.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
            });
      });
   }
 
   public static IApplicationBuilder UseCorsPolicies(this IApplicationBuilder app)
   {
      return app.UseCors();
   }
}