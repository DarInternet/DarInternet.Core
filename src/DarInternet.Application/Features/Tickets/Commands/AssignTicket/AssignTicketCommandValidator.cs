using FluentValidation;

namespace DarInternet.Application.Features.Tickets.Commands.AssignTicket
{
    public class AssignTicketCommandValidator: AbstractValidator<AssignTicketCommand>
    {
        public AssignTicketCommandValidator()
        {
            RuleFor(x=>x.UserId)
                .NotEmpty().NotNull();    

            RuleFor(x=>x.ConversationId)    
                .NotNull().NotEmpty();
        }
    }
}
