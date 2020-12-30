using DarInternet.Domain.Common;
using System.Threading.Tasks;

namespace DarInternet.Application.Common.Interfaces
{
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}
