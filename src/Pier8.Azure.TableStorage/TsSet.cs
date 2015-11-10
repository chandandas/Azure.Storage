using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage.Table
{
    public class TsSet<T> : ITsSet<T> where T : TableEntity, new()
    {
        private const int TABLE_BATCH_OPERATION_SIZE = 100;
        private const int MAX_BATCH_COUNT = 500;

        private readonly ITsTable<T> _table;

        public TsSet(ITsTable<T> table)
        {
            _table = table;
            _table.CreateTableIfNotExists();
        }
        
        public async Task<T> FindAsync(string partitionKey, string rowKey)
        {
            return await _table.Retrieve(partitionKey, rowKey);
        }

        public async Task AddAsync(T entity)
        {
            await _table.Insert(entity);
        }

        public Task UpdateAsync(T entity)
        {
            return _table.InsertOrMerge(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            await _table.Delete(entity);
        }

        /// <summary>
        /// Allows you to perform a query on the against the table.
        /// Warning this could result in a table scan being executed.
        /// </summary>
        /// <param name="query">A TsQuery object containing a TableQuery.</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> QueryAsync(TsQuery query)
        {
            var returnList = new List<T>();
            TableContinuationToken continuationToken = null;
            var tableQuery = new TableQuery<T>().Where(query.Query).Take(MAX_BATCH_COUNT);

            do
            {
                var tableQueryResult = await _table.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);
                returnList.AddRange(tableQueryResult.Results);
                continuationToken = tableQueryResult.ContinuationToken;
            }
            while (continuationToken != null);

            return returnList;
        }
        
        /// <summary>
        /// Adds a collection of etnities to the table in batches of 100.
        /// </summary>
        /// <param name="entities">A collection of entities to add.</param>
        /// <returns></returns>
        public async Task<BatchAddResult> BatchAddAsync(List<T> entities)
        {
            var batches = BatchIt(entities);

            foreach (var batch in batches)
            {
               await _table.BatchInsertOrMerge(batch);
            }

            return new BatchAddResult(batches.Count, entities.Count());
        }

        /// <summary>
        /// Retrieves a collection of entities based on the query and allows you to perform an action against each one.
        /// </summary>
        /// <param name="query">A TsQuery object containing a TableQuery.</param>
        /// <param name="action">The action to perform on each item returned by the query.</param>
        /// <returns></returns>
        public async Task<BatchProcessResult> BatchProcessAsync(TsQuery query, Func<List<T>, Task> action)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            if (action == null)
                throw new ArgumentNullException("action");

            TableContinuationToken continuationToken = null;
            var tableQuery = new TableQuery<T>().Where(query.Query).Take(MAX_BATCH_COUNT);
            var batches = 0;
            var entitiesProcessed = 0;

            do
            {
                var tableQueryResult = await _table.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);

                continuationToken = tableQueryResult.ContinuationToken;

                await action(tableQueryResult.Results);

                entitiesProcessed += tableQueryResult.Results.Count;
                batches++;
            }
            while (continuationToken != null);

            return new BatchProcessResult(batches, entitiesProcessed);
        }

        /// <summary>
        /// Deletes all the entites from the table based upon the provided query.
        /// </summary>
        /// <param name="query">A TsQuery object containing a TableQuery.</param>
        /// <returns></returns>
        public async Task<BatchDeleteResult> BatchDeleteAsync(TsQuery query)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            TableContinuationToken continuationToken = null;
            var tableQuery = new TableQuery<T>().Where(query.Query).Take(MAX_BATCH_COUNT);
            var batches = 0;
            var deleteBatches = 0;
            var entitiesDeleted = 0;

            do
            {
                var tableQueryResult = await _table.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);

                continuationToken = tableQueryResult.ContinuationToken;

                var results = await DeleteInBatchesAsync(tableQueryResult);
                deleteBatches += results.Item1;
                entitiesDeleted += results.Item2;
                batches++;
            }
            while (continuationToken != null);

            return new BatchDeleteResult(batches, deleteBatches, entitiesDeleted);
        }

        /// <summary>
        /// Allows you to update a collection of entites using the provided query.
        /// </summary>
        /// <param name="query">A TsQuery object containing a TableQuery.</param>
        /// <param name="action">The update action to perform on each item in the collection.</param>
        /// <returns></returns>
        public async Task<BatchUpdateResult> BatchUpdateAsync(TableQuery<T> query, Action<List<T>> action)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            if (action == null)
                throw new ArgumentNullException("action");

            TableContinuationToken continuationToken = null;
            var tableQuery = query.Take(MAX_BATCH_COUNT);
            var batches = 0;
            var updatedBatches = 0;
            var entitiesUpdated = 0;

            do
            {
                var tableQueryResult = await _table.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);

                continuationToken = tableQueryResult.ContinuationToken;

                action(tableQueryResult.Results);

                var results = await UpdateInBatchesAsync(tableQueryResult);
                updatedBatches += results.Item1;
                entitiesUpdated += results.Item2;
                batches++;
            }
            while (continuationToken != null);

            return new BatchUpdateResult(batches, updatedBatches, entitiesUpdated);
        }

        private async Task<Tuple<int, int>> UpdateInBatchesAsync(TsQueryResult<T> tableQueryResult)
        {
            var updateBatches = 0;
            var entitiesUpdated = 0;

            var batches = BatchIt(tableQueryResult.Results);

            foreach (var batch in batches)
            {
                updateBatches++;
                entitiesUpdated += batch.Count;

                await _table.BatchMergeAsync(batch);
            }

            return new Tuple<int, int>(updateBatches, entitiesUpdated);
        }

        private async Task<Tuple<int, int>> DeleteInBatchesAsync(TsQueryResult<T> tableQueryResult)
        {
            var deleteBatches = 0;
            var entitiesDeleted = 0;

            var batches = BatchIt(tableQueryResult.Results);

            foreach (var batch in batches)
            {
                deleteBatches++;
                entitiesDeleted += batch.Count;

                await _table.BatchDeleteAsync(batch);
            }

            return new Tuple<int, int>(deleteBatches, entitiesDeleted);
        }

        private List<List<T>> BatchIt(List<T> entities)
        {
            return entities
                .Select(e => new { e.PartitionKey, Entity = e })
                .GroupBy(e => e.PartitionKey)
                .Select(g => g.Select(l => l.Entity))
                .SelectMany(g => g.ChunkBy(TABLE_BATCH_OPERATION_SIZE))
                .ToList();
        }
    }
}