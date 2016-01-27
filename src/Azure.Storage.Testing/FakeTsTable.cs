using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using Azure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage.Testing
{
    public class FakeTsTable<T> : ITsTable<T> where T : TableEntity, new()
    {
        public FakeTsTable(IEnumerable<T> seed)
        {
            Entities = seed.ToDictionary(GetKey, s => s);
        }

        public Dictionary<Tuple<string, string>, T> Entities { get; private set; }

        public Task<T> Retrieve(string partitionKey, string rowKey)
        {
            var key = new Tuple<string, string>(partitionKey, rowKey);
            if (Entities.ContainsKey(key))
            {
                return Task.FromResult(Entities[key]);
            }

            return Task.FromResult(null as T);
        }

        public Task<TsQueryResult<T>> ExecuteQuerySegmentedAsync(TableQuery<T> tableQuery, TableContinuationToken continuationToken)
        {
            var queryString = tableQuery.FilterString
                                        .Replace(" eq ", "=")
                                        .Replace(" ne ", "!=")
                                        .Replace(" gt ", ">")
                                        .Replace(" lt ", "<")
                                        .Replace(" ge ", ">=")
                                        .Replace(" le ", "<=")
                                        .Replace("'", "\"");

            var skip = Convert.ToInt32(continuationToken == null ? "0" : continuationToken.NextPartitionKey);

            var results = Entities.Values.Where(queryString).Skip(skip).Take(500).ToList();

            TableContinuationToken returnContinuationToken = null;

            if (results.Any())
            {
                returnContinuationToken = new TableContinuationToken { NextPartitionKey = Convert.ToString(skip + 500) };
            }

            return Task.FromResult(new TsQueryResult<T>(results, returnContinuationToken));
        }

        public Task Insert(T entity)
        {
            var key = GetKey(entity);

            if (Entities.ContainsKey(key))
            {
                throw new Exception();
            }

            Entities.Add(key, entity);

            return Task.Delay(0);
        }

        public Task InsertOrMerge(T entity)
        {
            var key = GetKey(entity);

            if (Entities.ContainsKey(key))
            {
                Entities[key] = entity;
            }
            else
            {
                Entities.Add(key, entity);
            }

            return Task.Delay(0);
        }

        public Task Delete(T entity)
        {
            var key = GetKey(entity);

            if (Entities.ContainsKey(key))
            {
                Entities.Remove(key);
            }

            return Task.Delay(0);
        }

        public Task BatchInsertOrMerge(List<T> batch)
        {
            foreach (var entity in batch)
            {
                InsertOrMerge(entity);
            }

            return Task.Delay(0);
        }

        public Task BatchDeleteAsync(List<T> batch)
        {
            foreach (var entity in batch)
            {
                Delete(entity);
            }

            return Task.Delay(0);
        }

        public Task BatchMergeAsync(List<T> batch)
        {
            foreach (var entity in batch)
            {
                Merge(entity);
            }

            return Task.Delay(0);
        }

        private static Tuple<string, string> GetKey(TableEntity entity)
        {
            return new Tuple<string, string>(entity.PartitionKey, entity.RowKey);
        }

        private void Merge(T entity)
        {
            var key = GetKey(entity);

            if (Entities.ContainsKey(key))
            {
                Entities[key] = entity;
            }
        }
    }
}