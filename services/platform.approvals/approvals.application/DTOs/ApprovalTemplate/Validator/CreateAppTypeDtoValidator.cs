using FluentValidation;

namespace approvals.application.DTOs.ApplicationType.Validator
{
    public class CreateAppTypeDtoValidator : BaseAppTypeValidator<CreateApprovalTemplateDto>
    {
        public CreateAppTypeDtoValidator()
        {
            ApplyNameRules(RuleFor(v => v.Name));
            ApplyDescriptionRules(RuleFor(v => v.Description));
        }
    }
}
