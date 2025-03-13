using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Madarik.Common.Emailing.Mimekit;

internal static class MimekitModule
{
    
    public static IServiceCollection AddMimeKitEmailing(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IEmailSender, EmailSender>();
        return builder.Services;
    }
}