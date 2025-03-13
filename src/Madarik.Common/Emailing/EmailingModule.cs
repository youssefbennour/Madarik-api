using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Madarik.Common.Emailing.Mimekit;

namespace Madarik.Common.Emailing;

public static class EmailingModule
{
    public static IServiceCollection AddEmailingModule(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(EmailOptions.Key));
        builder.Services.AddSingleton(registeredServices => 
            registeredServices.GetRequiredService<IOptions<EmailOptions>>().Value);
        
        builder.AddMimeKitEmailing();

        return builder.Services;
    }
}