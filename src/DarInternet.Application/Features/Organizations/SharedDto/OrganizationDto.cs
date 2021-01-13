using System;
using DarInternet.Application.Common.Mappings;
using DarInternet.Domain.Entities;

namespace DarInternet.Application.Features.Organizations.SharedDto
{
    public class OrganizationDto : IMapFrom<Organization>
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

    }
}