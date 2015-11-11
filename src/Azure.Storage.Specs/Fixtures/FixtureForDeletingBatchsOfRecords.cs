using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Specs.TestClasses;
using Azure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage.Specs.Fixtures
{
    public class FixtureForDeletingBatchsOfRecords : TsContextBaseFixture, IDisposable
    {
        public BatchDeleteResult Result { get; private set; }

        public FixtureForDeletingBatchsOfRecords()
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

            var subject = new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));

            // ACT
            Result = subject.BatchDeleteAsync(new FindByPartitionKey("partitionKey")).Result;
        }

        public void Dispose()
        {
            DeleteTable();
        }
    }
}