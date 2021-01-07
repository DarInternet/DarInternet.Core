using System;
using DarInternet.Domain.Common;
using DarInternet.Domain.Enums;

namespace DarInternet.Domain.Entities
{
    public class OrganizationUser : AuditableEntity
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }

        public string UserId {get;set;}

        public UserType Type {get;set;}

        public virtual Organization Organization {get;set;}

        public virtual ApplicationUser User {get;set;}
    }
}