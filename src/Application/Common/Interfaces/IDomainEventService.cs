using Katsuretsu.Domain.Common;
using System.Threading.Tasks;

namespace Katsuretsu.Application.Common.Interfaces
{
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}
