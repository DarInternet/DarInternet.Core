using System;
using DarInternet.Domain.Common;
using System.Collections.Generic;

namespace DarInternet.Domain.Entities
{
    public class Organization : AuditableEntity
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public IList<OrganizationUser> OrganizationUsers { get; private set; } = new List<OrganizationUser>();

        public IList<Conversation> Conversations { get; private set; } = new List<Conversation>();

    }
}