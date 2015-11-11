using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Specs.TestClasses;
using Azure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage.Specs.Fixtures
{
    public class FixtureForProcessingABatchOfRecords : TsContextBaseFixture, IDisposable
    {
        public List<string> MyProperties { get; private set; }
        public BatchProcessResult Result { get; private set; }

        public FixtureForProcessingABatchOfRecords()
        {
            // ARRANGE
            TableClient = GetTableClient();
            Table = CreateTable();

            var tasks = Enumerable.Range(0, 600).Select(i =>
            {
                var op = TableOperation.Insert(new TestingEntity("partitionKey", Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
                return Table.ExecuteAsync(op);
            });

            Task.WhenAll(tasks).Wait();

            MyProperties = new List<string>();

            var subject = new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));

            // ACT
            Result = subject.BatchProcessAsync(
                new FindByPartitionKey("partitionKey"),
                entities =>
                {
                    MyProperties.AddRange(entities.Select(f => f.MyProperty));
                    return Task.Delay(0);
                }).Result;
        }
        
        public void Dispose()
        {
            DeleteTable();
        }
    }
}