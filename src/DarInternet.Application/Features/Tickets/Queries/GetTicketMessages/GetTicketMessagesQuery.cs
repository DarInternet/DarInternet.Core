using AutoMapper;
using AutoMapper.QueryableExtensions;
using DarInternet.Application.Common.Exceptions;
using DarInternet.Application.Common.Interfaces;
using DarInternet.Application.Common.Mappings;
using DarInternet.Application.Common.Models;
using DarInternet.Application.Features.Tickets.SharedDto;
using DarInternet.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DarInternet.Application.Features.Tickets.Queries.GetTicketMessages
{
    public class GetTicketMessagesQuery: IRequest<PaginatedList<TicketMessageDto>>
    {
        public Guid ConversationId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        
    }

    //ToDo: should write integreation tests for this 
    public class Handler : IRequestHandler<GetTicketMessagesQuery, PaginatedList<TicketMessageDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public Handler(IApplicationDbContext context, IMapper mapper, ICurrentUserService currentUser)
        {
            _context = context;
            _mapper = mapper;
            _currentUser= currentUser;
        }

        public async Task<PaginatedList<TicketMessageDto>> Handle(GetTicketMessagesQuery request, CancellationToken cancellationToken)
        {
            await ValidateRequest(request);
            
            return await _context.ConversationMessages
                .Where(x => x.ConversationId == request.ConversationId)
                .OrderBy(x=>x.Created)
                .ProjectTo<TicketMessageDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize); ;
        }

        private async Task ValidateRequest(GetTicketMessagesQuery request)
        
        {
            if (await _context.Conversations.AsNoTracking().AnyAsync(x => x.Id == request.ConversationId) == false)
            {
                throw new NotFoundException(nameof(Conversation), request.ConversationId);
            }

            if (await _context.ConversationUsers.AsNoTracking().AnyAsync(x => x.ConversationId == request.ConversationId && x.UserId == _currentUser.UserId && x.IsDisabled==false) == false)
            {
                throw new ForbiddenAccessException();
            }
        }
    }
}
