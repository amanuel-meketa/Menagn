using FluentValidation;

namespace approvals.application.DTOs.CommonDTOs.Validator
{
    public abstract class BaseAppTypeValidator<T> : AbstractValidator<T>
    {
        protected void ApplyGuidIdRules(IRuleBuilder<T, Guid> rule)
        {
            rule.NotEmpty().WithMessage("{PropertyName} is required.")
                .Must(id => id != Guid.Empty).WithMessage("{PropertyName} must be a valid GUID.");
        }

        protected void ApplyNameRules(IRuleBuilder<T, string> rule)
        {
            rule.NotEmpty().WithMessage("{PropertyName} is required.")
                .MaximumLength(64).WithMessage("{PropertyName} must not exceed {MaxLength} characters.");
        }

        protected void ApplyDescriptionRules(IRuleBuilder<T, string> rule)
        {
            rule.NotEmpty().WithMessage("{PropertyName} is required.")
                .MaximumLength(500).WithMessage("{PropertyName} must not exceed {MaxLength} characters.");
        }
    }
}
