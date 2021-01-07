using System;
using DarInternet.Domain.Common;

namespace DarInternet.Domain.Entities
{
    public class ConversationMessage : AuditableEntity
    {
        public Guid Id { get; set; }

        public Guid ConversationId { get; set; }
        
        public string UserId { get; set; }
        
        public string Message { get; set; }

        public virtual Conversation Conversation {get;set;}

        public virtual ApplicationUser User {get;set;}

    }
}