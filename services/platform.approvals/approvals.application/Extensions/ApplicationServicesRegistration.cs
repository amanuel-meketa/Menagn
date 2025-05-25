using Microsoft.Extensions.DependencyInjection;

namespace approvals.application
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection ConfigurApplicationServices(this IServiceCollection service)
        {
            return service.AddAutoMapper(typeof(ApplicationServicesRegistration).Assembly);
        }
    }
}
