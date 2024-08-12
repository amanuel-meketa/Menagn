using Microsoft.Extensions.DependencyInjection;

namespace security.sharedUtils
{
    public static class SharedServicesRegistration
    {
        public static IServiceCollection ConfigurApplicationServices(this IServiceCollection service)
        {
            return service.AddAutoMapper(typeof(SharedServicesRegistration).Assembly); ;
        }
    }
}
