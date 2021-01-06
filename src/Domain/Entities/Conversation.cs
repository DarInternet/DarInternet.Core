using System;
using System.Collections.Generic;
using DarInternet.Domain.Common;

namespace DarInternet.Domain.Entities
{
    public class Conversation : AuditableEntity
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }

        public string Title { get; set; }

        public bool IsClosed { get; set; }

        public virtual Organization Organization {get;set;}

        public IList<ConversationUser> ConversationUsers { get; private set; } = new List<ConversationUser>();

        public IList<ConversationMessage> ConversationMessages { get; private set; } = new List<ConversationMessage>();


    }
}