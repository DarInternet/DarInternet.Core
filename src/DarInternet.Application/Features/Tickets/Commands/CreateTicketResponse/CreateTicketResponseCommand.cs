using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DarInternet.Application.Common.Exceptions;
using DarInternet.Application.Common.Interfaces;
using DarInternet.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DarInternet.Application.Features.Tickets.Commands.CreateTicketResponse
{
    public class CreateTicketResponseCommand : IRequest<Guid>
    {
        public Guid ConversationId { get; set; }

        public string Message { get; set; }

        
    }

    public class Handler : IRequestHandler<CreateTicketResponseCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        public Handler(IApplicationDbContext context,ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser=currentUser;
        }
       
        public async Task<Guid> Handle(CreateTicketResponseCommand request, CancellationToken cancellationToken)
        {
            await ValidateRequest(request);

            ConversationMessage conversationMessage = await CreateConversationMessage(request, cancellationToken);

            return conversationMessage.Id;
        }

        private async Task<ConversationMessage> CreateConversationMessage(CreateTicketResponseCommand request, CancellationToken cancellationToken)
        {
            var conversationMessage = new ConversationMessage
            {
                ConversationId = request.ConversationId,
                UserId = _currentUser.UserId,
                Message = request.Message
            };

            await _context.ConversationMessages.AddAsync(conversationMessage);

            await _context.SaveChangesAsync(cancellationToken);
            return conversationMessage;
        }

        private async Task ValidateRequest(CreateTicketResponseCommand request)
        
        {
            var conversation = await _context.Conversations.AsNoTracking().Select(x => new { x.Id, x.IsClosed }).SingleOrDefaultAsync(x => x.Id == request.ConversationId);
            if (conversation == null)
            {
                throw new NotFoundException(nameof(Conversation), request.ConversationId);
            }
            if (conversation.IsClosed)
            {
                throw new Exception("You can't reply to closed ticket");
            }
            if (await _context.ConversationUsers.AsNoTracking().AnyAsync(x => x.ConversationId == request.ConversationId && x.UserId == _currentUser.UserId) == false)
            {
                throw new ForbiddenAccessException();
            }
        }
    }
}