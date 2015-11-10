using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage.Table
{
    public interface ITsTable<T> where T : TableEntity, new()
    {
        void CreateTableIfNotExists();

        Task<T> Retrieve(string partitionKey, string rowKey);

        Task Insert(T entity);

        Task InsertOrMerge(T entity);

        Task Delete(T entity);

        Task<TsQueryResult<T>> ExecuteQuerySegmentedAsync(TableQuery<T> tableQuery, TableContinuationToken continuationToken);

        Task BatchInsertOrMerge(List<T> batch);

        Task BatchDeleteAsync(List<T> batch);

        Task BatchMergeAsync(List<T> batch);
    }
}