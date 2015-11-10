using System.Collections.Generic;
using System.Threading.Tasks;

namespace Azure.Storage.Table
{
    public interface ITsSet<T>
    {
        Task<T> FindAsync(string partitionKey, string rowKey);

        Task AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);

        Task<IEnumerable<T>> QueryAsync(TsQuery query);
    }
}