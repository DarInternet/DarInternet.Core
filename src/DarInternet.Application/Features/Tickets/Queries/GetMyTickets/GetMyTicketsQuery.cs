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

namespace DarInternet.Application.Features.Tickets.Queries.GetMyTickets
{
    public class GetMyTicketsQuery: IRequest<PaginatedList<TicketDto>>
    {
        public Guid OrganizationId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        
    }

    //ToDo: should write integreation tests for this 
    public class Handler : IRequestHandler<GetMyTicketsQuery, PaginatedList<TicketDto>>
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

        public async Task<PaginatedList<TicketDto>> Handle(GetMyTicketsQuery request, CancellationToken cancellationToken)
        {
            await ValidateRequest(request);
            
            return await _context.Conversations.Include(x => x.ConversationUsers)
                .Where(x => x.OrganizationId == request.OrganizationId && x.ConversationUsers.Any(y =>y.IsDisabled==false && y.ConversationId == x.Id && y.UserId == _currentUser.UserId))
                .ProjectTo<TicketDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize); ;
        }

        private async Task ValidateRequest(GetMyTicketsQuery request)
        
        {
            if (await _context.Organizations.AsNoTracking().AnyAsync(x => x.Id == request.OrganizationId) == false)
            {
                throw new NotFoundException(nameof(Organization), request.OrganizationId);
            }

            if (await _context.OrganizationUsers.AsNoTracking().AnyAsync(x => x.OrganizationId == request.OrganizationId && x.UserId == _currentUser.UserId) == false)
            {
                throw new ForbiddenAccessException();
            }
        }
    }
}
