using Authorization.Services;
using Authorization.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization.UnitTest
{
    public class DIServices
    {
        public ServiceProvider GenerateDependencyInjection()
        {
            var services = new ServiceCollection();
            services.AddScoped(typeof(IPaginationService<>), typeof(PaginationService<>));

            return services
                .AddLogging()
                .BuildServiceProvider();
        }
    }
}