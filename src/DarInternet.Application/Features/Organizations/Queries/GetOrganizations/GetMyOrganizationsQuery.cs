using AutoMapper;
using AutoMapper.QueryableExtensions;
using DarInternet.Application.Common.Interfaces;
using DarInternet.Application.Common.Mappings;
using DarInternet.Application.Common.Models;
using DarInternet.Application.Features.Organizations.SharedDto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DarInternet.Application.Features.Organizations.Queries.GetOrganizations
{
    public class GetMyOrganizationsQuery: IRequest<PaginatedList<OrganizationDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        
    }

    public class Handler : IRequestHandler<GetMyOrganizationsQuery, PaginatedList<OrganizationDto>>
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

        public async Task<PaginatedList<OrganizationDto>> Handle(GetMyOrganizationsQuery request, CancellationToken cancellationToken)
        {
            var myOrganizationIds = await _context.OrganizationUsers
                                                  .AsNoTracking()
                                                  .Select(x=>new {x.OrganizationId , x.UserId})
                                                  .Where(x=>x.UserId == _currentUser.UserId)
                                                  .Select(x=>x.OrganizationId)
                                                  .Distinct()
                                                  .ToListAsync();
             
            return await _context.Organizations
                .Where(x => myOrganizationIds.Contains(x.Id))
                .ProjectTo<OrganizationDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize); ;
        }
    }
}
