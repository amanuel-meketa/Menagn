using approvals.application.DTOs.CommonDTOs.Validator;
using FluentValidation;

namespace approvals.application.DTOs.ApplicationType.Validator
{
    public class UpdateAppTypeDtoValidator : BaseAppTypeValidator<UpdatAppemplateDto>
    {
        public UpdateAppTypeDtoValidator()
        {
            ApplyGuidIdRules(RuleFor(v => v.Id));
            ApplyNameRules(RuleFor(v => v.Name));
            ApplyDescriptionRules(RuleFor(v => v.Description));
        }
    }
}
