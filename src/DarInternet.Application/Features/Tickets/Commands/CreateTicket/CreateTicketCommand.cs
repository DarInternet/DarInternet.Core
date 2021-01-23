using System;
using System.Threading;
using System.Threading.Tasks;
using DarInternet.Application.Common.Interfaces;
using DarInternet.Domain.Entities;
using MediatR;
using DarInternet.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DarInternet.Application.Features.Tickets.Commands.CreateTicket
{
    public class CreateTicketCommand: IRequest<Guid>
    {
        public Guid OrganizationId { get; set; }
        
        public string Title { get; set; } 

        public string Message { get; set; }       
    }

    public class Handler : IRequestHandler<CreateTicketCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        public Handler(IApplicationDbContext context,ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser=currentUser;
        }
       
        public async Task<Guid> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            await ValidateRequest(request);

            Conversation conversation = await CreateConversation(request, cancellationToken);

            await CreateConversationUser(conversation, cancellationToken);

            await CreateConversationMessage(request, conversation, cancellationToken);

            return conversation.Id;
        }

        private async Task ValidateRequest(CreateTicketCommand request)
        {
            if (await _context.Organizations.AsNoTracking().AnyAsync(x => x.Id == request.OrganizationId)==false)
            {
                throw new NotFoundException(nameof(Organization), request.OrganizationId);
            }
            if (await _context.OrganizationUsers.AsNoTracking().AnyAsync(x => x.UserId == _currentUser.UserId && x.OrganizationId == request.OrganizationId) == false)
            {
                throw new ForbiddenAccessException();
            }
        }

        private async Task CreateConversationMessage(CreateTicketCommand request, Conversation conversation, CancellationToken cancellationToken)
        {          

            var conversationMessage = new ConversationMessage
            {
                ConversationId = conversation.Id,
                Message = request.Message,
                UserId = _currentUser.UserId
            };

            await _context.ConversationMessages.AddAsync(conversationMessage);

            await _context.SaveChangesAsync(cancellationToken);             
             
        }

        
        private async Task CreateConversationUser(Conversation conversation, CancellationToken cancellationToken)
        {
            var conversationUser = new ConversationUser
            {
                ConversationId = conversation.Id,
                UserId = _currentUser.UserId,
                IsDisabled = false
            };

            await _context.ConversationUsers.AddAsync(conversationUser);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task<Conversation> CreateConversation(CreateTicketCommand request, CancellationToken cancellationToken)
        {        
            var conversation = new Conversation
            {
                Title = request.Title,
                IsClosed = false,
                OrganizationId = request.OrganizationId
            };

            await _context.Conversations.AddAsync(conversation);

            await _context.SaveChangesAsync(cancellationToken);
            return conversation;
        }
    }
}