using Common.Utils;
using System.Linq.Expressions;

namespace Common.Base;

public interface IReadRepository<T> where T : AggregateRoot
{
    Task<T> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<PagedResult<T>> PagedAsync(PaginationRequest paginationRequest, Expression<Func<T, bool>> predicate = null!);
}
