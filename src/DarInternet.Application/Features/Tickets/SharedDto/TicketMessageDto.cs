using System;
using DarInternet.Application.Common.Mappings;
using DarInternet.Domain.Entities;

namespace DarInternet.Application.Features.Tickets.SharedDto
{
    public class TicketMessageDto : IMapFrom<ConversationMessage>
    {
        public Guid Id { get; set; }

        public Guid ConversationId { get; set; }
        
        public string UserId { get; set; }
        
        public string Message { get; set; }

        public DateTime Created { get; set; }
    }
}