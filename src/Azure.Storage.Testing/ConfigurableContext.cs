using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Azure.Storage.Core;
using Azure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage.Testing
{
    public class ConfigurableContext<T>  where T : new()
    {
        public readonly T Context;
        private readonly bool _useTableStorage;
        private readonly Dictionary<Type, object> _data;

        public ConfigurableContext(Action<ConfigurableContext<T>> configuration, bool useTableStorage = false)
        {
            _useTableStorage = useTableStorage;
            _data = new Dictionary<Type, object>();
            Context = new T();
            configuration.Invoke(this);
        }

        public static implicit operator T(ConfigurableContext<T> configurableContext)
        {
            return configurableContext.Context;
        }

        public void Setup<TU>(Expression<Func<T, TsSet<TU>>> property) where TU : TableEntity, new()
        {
            Setup(property, new List<TU>());
        }

        public void Setup<TU>(Expression<Func<T, TsSet<TU>>> property, List<TU> seed) where TU : TableEntity, new()
        {
            if(_useTableStorage)
            {
                SetRealTsTableData(property, seed);
            }
            else
            {
                SetFakeTsTableData(property, seed);
            }
        }

        private void SetRealTsTableData<TU>(Expression<Func<T, TsSet<TU>>> property, List<TU> seed) where TU : TableEntity, new()
        {
            var member = property.Body as MemberExpression;
            var propertyInfo = member.Member as PropertyInfo;

            var table = GetCloudTable(propertyInfo.Name);
            var tsTable = new TsTable<TU>(table);

            tsTable.DeleteAllAsync().Wait();
            tsTable.BatchInsertOrMerge(seed).Wait();

            propertyInfo.SetValue(Context, new TsSet<TU>(tsTable));
        }

        private CloudTable GetCloudTable(string name)
        {
            var storageAccount = CloudStorageAccount
                .Parse(ConfigurationManager.AppSettings["DataConnectionString"]);

            var tableClient = storageAccount.CreateCloudTableClient();

            var tableReference = tableClient.GetTableReference(name);

            // Create if not exist
            tableReference.CreateIfNotExistsAsync().Wait();
            return tableReference;
        }

        private void SetFakeTsTableData<TU>(Expression<Func<T, TsSet<TU>>> property, List<TU> seed) where TU : TableEntity, new()
        {
            var member = property.Body as MemberExpression;
            var propertyInfo = member.Member as PropertyInfo;

            var fakeTsTable = new FakeTsTable<TU>(seed);

            if(_data.ContainsKey(typeof(TU)))
            {
                _data.Remove(typeof(TU));
            }

            _data.Add(typeof(TU), fakeTsTable);

            propertyInfo.SetValue(Context, new TsSet<TU>(fakeTsTable));
        }

        public void HasBeenSaved<TU>(Expression<Func<TU, bool>> assertions) where TU : TableEntity, new()
        {
            var exactResults = GetList<TU>();
            ExpressionAnalyser.CheckMessagesMatch(exactResults, assertions);
        }

        public void HasNotBeenSaved<TU>(Expression<Func<TU, bool>> assertions) where TU : TableEntity, new()
        {
            var results = GetList<TU>();
            ExpressionAnalyser.CheckMessagesDoNotMatch(results, assertions);
        }

        public void HasBeenDeleted<TU>(Expression<Func<TU, bool>> assertions) where TU : TableEntity, new()
        {
            var results = GetList<TU>();
            ExpressionAnalyser.CheckMessagesDoNotMatch(results, assertions);
        }

        private List<TU> GetList<TU>() where TU : TableEntity, new()
        {
            if(_useTableStorage)
            {
                return GetRealList<TU>();
            }
            else
            {
                return GetFakeList<TU>();
            }
            
        }

        private List<TU> GetRealList<TU>() where TU : TableEntity, new()
        {
            var table = GetCloudTable(typeof(TU).Name);
            var tsTable = new TsTable<TU>(table);
            TableContinuationToken continuationToken = null;
            List < TU > items = new List<TU>();

            do
            {
                var results = tsTable.ExecuteQuerySegmentedAsync(new TableQuery<TU>(), continuationToken).Result;
                items.AddRange(results.Results);
                continuationToken = results.ContinuationToken;
            }
            while (continuationToken != null);

            return items;
        }

        private List<TU> GetFakeList<TU>() where TU : TableEntity, new()
        {
            if(!_data.ContainsKey(typeof(TU)))
            {
                throw new Exception();
            }

            var fakeTsTable = _data[typeof(TU)] as FakeTsTable<TU>;

            return fakeTsTable.Entities.Select(e => e.Value).ToList();
        }
    }
}