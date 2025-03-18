using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Madarik.Common.ApiConfiguration;
using Madarik.Common.Auth;
using Madarik.Common.Clocks;
using Madarik.Common.Emailing;
using Madarik.Common.ErrorHandling;
using Madarik.Common.Events.Publisher;
using Madarik.Common.Localizations;
using Madarik.Common.Telemetry;
using Madarik.Madarik;
using Madarik.Madarik.Data.Database;
using Madarik.Madarik.Data.Users;

var builder = WebApplication.CreateBuilder(args);
builder.AddExceptionHandling()
    .AddTelemetry();

builder.AddAuthModule();
builder.AddEmailingModule();

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<SalamHackPersistence>()
    .AddApiEndpoints();

builder.Services
    .AddApiConfiguration<Program>()
    .AddClock()
    .AddRequestBasedLocalization()
    .AddPublisher(Assembly.GetExecutingAssembly());


builder.UseOrleans((silo) =>
{
    silo.UseLocalhostClustering();
    silo.AddMemoryGrainStorage("VolumesStorage");
});


builder.Services
    .AddContracts(builder.Configuration);

var app = builder.Build();

if(app.Environment.IsDevelopment()) {
    app.UseSwagger();
}

app.UseApiConfiguration();
app.UseExceptionHandling();
app.UseAuthModule();
app.UseRequestBasedLocalization();
app.MapControllers();
app.UseHttpLogging();
app.UseTelemetry();

app.MapIdentityApi<User>();
app.UseContracts();

app.MapContracts();
app.MapLocalizationSampleEndpoint();
#pragma warning disable S6966
app.Run();

namespace Madarik {
    [UsedImplicitly]
    public sealed class Program;
}
