using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Madarik.Common.Events.Publisher.InMemory;

namespace Madarik.Common.Events.Publisher;

public static class PublisherModule 
{
    public static IServiceCollection AddPublisher(this IServiceCollection services, Assembly assembly) =>
        services.AddInMemoryPublisher(assembly);
}