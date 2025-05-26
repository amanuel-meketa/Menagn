using FluentValidation;

namespace approvals.application.DTOs.ApplicationType.Validator
{
    public class CreateAppTypeDtoValidator : AbstractValidator<CreateApplicationTypeDto>
    {
        public CreateAppTypeDtoValidator()
        {
            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .MaximumLength(5).WithMessage("{PropertyName} must not exceed {MaxLength} characters.");

            RuleFor(v => v.Description)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .MaximumLength(500).WithMessage("{PropertyName} must not exceed {MaxLength} characters.");
        }
    }
}
