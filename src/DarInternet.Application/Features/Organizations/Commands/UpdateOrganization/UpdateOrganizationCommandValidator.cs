using FluentValidation;

namespace DarInternet.Application.Features.Organizations.Commands.UpdateOrganization
{
    public class UpdateOrganizationCommandValidator: AbstractValidator<UpdateOrganizationCommand>
    {
        public UpdateOrganizationCommandValidator()
        {
            RuleFor(v => v.Title)
                .MaximumLength(200)
                .NotEmpty().NotNull();
        }
    }
}
