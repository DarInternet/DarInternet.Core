using System;
using System.Threading;
using System.Threading.Tasks;
using DarInternet.Application.Common.Interfaces;
using DarInternet.Domain.Entities;
using MediatR;

namespace DarInternet.Application.Features.Organizations.Commands.CreateOrganization
{
    public class CreateOrganizationCommand : IRequest<Guid>
    {
        public string Title { get; set; }        
    }

    public class Handler : IRequestHandler<CreateOrganizationCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        public Handler(IApplicationDbContext context,ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser=currentUser;
        }
       
        public async Task<Guid> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {
            Organization organization = await CreateOrgainzation(request, cancellationToken);
            
            await CreateOrganizationUser(organization, cancellationToken);

            return organization.Id;
        }

        
        private async Task CreateOrganizationUser(Organization organization, CancellationToken cancellationToken)
        {
            var organizationUser = new OrganizationUser
            {
                OrganizationId = organization.Id,
                UserId = _currentUser.UserId,
                Type = Domain.Enums.UserType.Admin
            };

            await _context.OrganizationUsers.AddAsync(organizationUser);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task<Organization> CreateOrgainzation(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {        
            var organization = new Organization
            {
                Title = request.Title
            };

            await _context.Organizations.AddAsync(organization);

            await _context.SaveChangesAsync(cancellationToken);
            return organization;
        }
    }
}