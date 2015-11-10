using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage.Table
{
    public class TsTable<T> : ITsTable<T> where T : TableEntity, new()
    {
        private readonly CloudTable _cloudTable;

        public TsTable(CloudTable cloudTable)
        {
            _cloudTable = cloudTable;
        }

        public void CreateTableIfNotExists()
        {
            _cloudTable.CreateIfNotExists();
        }

        public async Task<T> Retrieve(string partitionKey, string rowKey)
        {
            var result = await _cloudTable.ExecuteAsync(TableOperation.Retrieve<T>(partitionKey, rowKey));
            return result.Result as T;
        }

        public async Task Insert(T entity)
        {
            await _cloudTable.ExecuteAsync(TableOperation.Insert(entity));
        }

        public async Task InsertOrMerge(T entity)
        {
            await _cloudTable.ExecuteAsync(TableOperation.InsertOrMerge(entity));
        }

        public async Task Delete(T entity)
        {
            try
            {
                await _cloudTable.ExecuteAsync(TableOperation.Delete(entity));
            }
            catch(Exception)
            {
                // ignored as this is to catch 404 Table Row does not exist.
            }
        }

        public async Task<TsQueryResult<T>> ExecuteQuerySegmentedAsync(TableQuery<T> tableQuery, TableContinuationToken continuationToken)
        {
            var result = await _cloudTable.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);
            return new TsQueryResult<T>(result.Results, result.ContinuationToken);
        }

        public async Task BatchInsertOrMerge(List<T> batch)
        {
            var tableBatchOperation = new TableBatchOperation();

            tableBatchOperation.AddRange(batch.Select(TableOperation.InsertOrMerge));

            await _cloudTable.ExecuteBatchAsync(tableBatchOperation);
        }

        public async Task BatchDeleteAsync(List<T> batch)
        {
            var tableBatchOperation = new TableBatchOperation();

            tableBatchOperation.AddRange(batch.Select(TableOperation.Delete));

            await _cloudTable.ExecuteBatchAsync(tableBatchOperation);
        }

        public async Task BatchMergeAsync(List<T> batch)
        {
            var tableBatchOperation = new TableBatchOperation();

            tableBatchOperation.AddRange(batch.Select(TableOperation.Merge));

            await _cloudTable.ExecuteBatchAsync(tableBatchOperation);
        }

        public async Task DeleteAllAsync()
        {
            await _cloudTable.DeleteIfExistsAsync();
            await _cloudTable.CreateIfNotExistsAsync();
        }
    }
}