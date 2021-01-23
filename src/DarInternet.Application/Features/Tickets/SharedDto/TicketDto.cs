using Microsoft.VisualBasic.CompilerServices;
using DarInternet.Application.Common.Mappings;
using DarInternet.Domain.Entities;
using System;

namespace DarInternet.Application.Features.Tickets.SharedDto
{
    public class TicketDto : IMapFrom<Conversation>
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }

        public string Title { get; set; }

        public bool IsClosed { get; set; }
    }
}