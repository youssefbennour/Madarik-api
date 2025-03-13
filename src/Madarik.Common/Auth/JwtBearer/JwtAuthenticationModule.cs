using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Madarik.Common.Auth.JwtBearer;

public static class JwtAuthenticationModule
{
    public static WebApplicationBuilder AddJwtAuthentication(this WebApplicationBuilder builder)
    {
       
        builder.Services.AddAuthentication()
            .AddCookie(IdentityConstants.ApplicationScheme)
            .AddBearerToken(IdentityConstants.BearerScheme);
            
        return builder;
    }

    public static IApplicationBuilder UseJwtAuth(this WebApplication app)
    {
        app.UseAuthentication();
        return app;
    }   
}