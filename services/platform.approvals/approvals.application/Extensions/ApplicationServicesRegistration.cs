using approvals.application.DTOs.ApplicationType.Validator;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace approvals.application
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection ConfigurApplicationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateAppTypeDtoValidator>(); // Register your validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(typeof(ApplicationServicesRegistration).Assembly);

            return services;
        }
    }
}
