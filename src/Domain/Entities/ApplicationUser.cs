using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
namespace DarInternet.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public IList<OrganizationUser> OrganizationUsers { get; private set; } = new List<OrganizationUser>();

        public IList<ConversationUser> ConversationUsers { get; private set; } = new List<ConversationUser>();

        public IList<ConversationMessage> ConversationMessages { get; private set; } = new List<ConversationMessage>();
        
    }
}