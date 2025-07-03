namespace Common.Base;

public interface IWriteRepository<T> where T : AggregateRoot
{
    Task<T> AddAsync(T entity);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity);
    Task UpdateRangeAsync(IEnumerable<T> entity);
    Task DeleteAsync(string id);
    Task DeleteRangeAsync(IEnumerable<string> ids);
}
