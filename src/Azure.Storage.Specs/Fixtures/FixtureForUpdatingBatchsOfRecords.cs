using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Specs.TestClasses;
using Azure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage.Specs.Fixtures
{
    public class FixtureForUpdatingBatchsOfRecords : TsContextBaseFixture, IDisposable
    {
        public BatchUpdateResult Result { get; private set; }

        public FixtureForUpdatingBatchsOfRecords()
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

            var tableQuery = new TableQuery<TestingEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "partitionKey"));

            var subject = new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));

            // ACT
            Result = subject.BatchUpdateAsync(
                tableQuery,
                entities =>
                {
                    entities.ForEach(e => e.MyProperty = "Test");
                }).Result;
        }

        public void Dispose()
        {
            DeleteTable();
        }
    }
}