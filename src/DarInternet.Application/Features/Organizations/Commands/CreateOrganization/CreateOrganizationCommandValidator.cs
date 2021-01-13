using FluentValidation;

namespace DarInternet.Application.Features.Organizations.Commands.CreateOrganization
{
    public class CreateOrganizationCommandValidator: AbstractValidator<CreateOrganizationCommand>
    {
        public CreateOrganizationCommandValidator()
        {
            RuleFor(v => v.Title)
                .MaximumLength(200)
                .NotEmpty().NotNull();
        }
    }
}
