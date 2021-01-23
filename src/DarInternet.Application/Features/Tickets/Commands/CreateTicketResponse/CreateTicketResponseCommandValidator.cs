using FluentValidation;

namespace DarInternet.Application.Features.Tickets.Commands.CreateTicketResponse
{
    public class CreateTicketResponseCommandValidator: AbstractValidator<CreateTicketResponseCommand>
    {
        public CreateTicketResponseCommandValidator()
        {           
            RuleFor(x=>x.Message)
                .NotEmpty().NotNull();   

            RuleFor(x=>x.ConversationId)
                .NotNull().NotEmpty();     
        }
    }
}
