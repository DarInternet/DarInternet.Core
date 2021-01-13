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

namespace DarInternet.Application.Features.Organizations.Commands.UpdateOrganization
{
    public class UpdateOrganizationCommand: IRequest
    {
        public Guid OrganizationId { get; set; }
        public string Title { get; set; }        
    }

    public class Handler : IRequestHandler<UpdateOrganizationCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        public Handler(IApplicationDbContext context,ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser=currentUser;
        }
       
        public async Task<Unit> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            Organization organization = await _context.Organizations.FindAsync(request.OrganizationId);

            if (organization==null)
            {
                throw new NotFoundException(nameof(Organization),request.OrganizationId);
            }

            if (await _context.OrganizationUsers
                              .AsNoTracking()
                              .Select(x=>new {x.OrganizationId, x.UserId , x.Type})
                              .AnyAsync(x=>x.OrganizationId==request.OrganizationId && 
                                           x.UserId == _currentUser.UserId && 
                                           x.Type== UserType.Admin)==false)
            {
                throw new ForbiddenAccessException();
            }
            
            organization.Title = request.Title;

            _context.Organizations.Update(organization);

            await _context.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }

       
    }
}