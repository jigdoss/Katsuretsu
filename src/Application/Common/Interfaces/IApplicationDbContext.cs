using System.Threading;
using System.Threading.Tasks;

namespace Katsuretsu.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
