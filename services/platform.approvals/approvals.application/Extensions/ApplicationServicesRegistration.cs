using approvals.application.DTOs.ApplicationType.Validator;
using approvals.application.Interfaces.Services;
using approvals.application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace approvals.application
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection ConfigurApplicationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateAppTypeDtoValidator>();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(typeof(ApplicationServicesRegistration).Assembly);
            services.AddScoped<IApprovalTemplateService, ApprovalTemplateService>();
            services.AddScoped<IStageDefinitionService, StageDefinitionService>();
            services.AddScoped<IApprovalInstanceService, ApprovalInstanceService>();
            services.AddScoped<IStageInstanceService, StageInstanceService>();

            return services;
        }
    }
}
