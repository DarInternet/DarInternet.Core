using FluentValidation;

namespace DarInternet.Application.Features.Tickets.Commands.CreateTicket
{
    public class CreateTicketCommandValidator: AbstractValidator<CreateTicketCommand>
    {
        public CreateTicketCommandValidator()
        {
            RuleFor(v => v.Title)
                .MaximumLength(1000)
                .NotEmpty().NotNull();

            RuleFor(x=>x.Message)
                .NotEmpty().NotNull();    

            RuleFor(x=>x.OrganizationId)    
                .NotNull().NotEmpty();
        }
    }
}
