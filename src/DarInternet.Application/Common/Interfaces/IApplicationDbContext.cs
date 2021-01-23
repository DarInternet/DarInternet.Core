using DarInternet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace DarInternet.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Organization> Organizations {get;set;}

        DbSet<OrganizationUser> OrganizationUsers {get;set;}

        DbSet<Conversation> Conversations {get;set;}

        DbSet<ConversationUser> ConversationUsers {get;set;}

        DbSet<ConversationMessage> ConversationMessages {get;set;}

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
