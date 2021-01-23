using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DarInternet.Application.Common.Exceptions;
using DarInternet.Application.Common.Interfaces;
using DarInternet.Domain.Entities;
using DarInternet.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DarInternet.Application.Features.Tickets.Commands.AssignTicket
{
    public class AssignTicketCommand : IRequest
    {
        public Guid ConversationId { get; set; }

        public string UserId { get; set; }

        public bool ShouldRemovePreviousUsers { get; set; }

    }

    public class Handler : IRequestHandler<AssignTicketCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        public Handler(IApplicationDbContext context,ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser=currentUser;
        }
       
        public async Task<Unit> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
        {
            await ValidateRequest(request);

            if (request.ShouldRemovePreviousUsers)
            {
                await RemovePreviousUsers(request, cancellationToken);
            }

            await AssignTicketToUser(request, cancellationToken);

            return Unit.Value;
        }

        private async Task AssignTicketToUser(AssignTicketCommand request, CancellationToken cancellationToken)
        {
            var conversationUser = new ConversationUser
            {
                ConversationId = request.ConversationId,
                UserId = _currentUser.UserId,
                IsDisabled = false
            };

            await _context.ConversationUsers.AddAsync(conversationUser);

            await _context.SaveChangesAsync(cancellationToken);
        }

        //ToDo: This shouldn't disable customer userId
        private async Task RemovePreviousUsers(AssignTicketCommand request, CancellationToken cancellationToken)
        {
            var previousConversationUsers = await _context.ConversationUsers.Where(x => x.ConversationId == request.ConversationId).ToListAsync();
            foreach (var prvConversationUser in previousConversationUsers)
            {
                prvConversationUser.IsDisabled = true;

                _context.ConversationUsers.Update(prvConversationUser);
            }
            await _context.SaveChangesAsync(cancellationToken);
        }


        private async Task ValidateRequest(AssignTicketCommand request)
        
        {
            var conversation = await _context.Conversations.AsNoTracking().Select(x => new { x.Id, x.IsClosed , x.OrganizationId }).SingleOrDefaultAsync(x => x.Id == request.ConversationId);
            if (conversation == null)
            {
                throw new NotFoundException(nameof(Conversation), request.ConversationId);
            }
            if (conversation.IsClosed)
            {
                throw new Exception("You can't assign new user to closed ticket");
            }
            if (await _context.OrganizationUsers.AsNoTracking().AnyAsync(x=>x.UserId==_currentUser.UserId && x.OrganizationId==conversation.OrganizationId && x.Type!=UserType.Customer)==false)
            {
                throw new ForbiddenAccessException();
            }
            
        }
    }
}