using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Specs.TestClasses;
using Azure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage.Specs.Fixtures
{
    using Testing;

    public class FixtureForUpdatingBatchsOfRecords
    {
        private FakeTsTable<TestingEntity> _fakeTsTable;
        public BatchUpdateResult Result { get; private set; }

        public FakeTsTable<TestingEntity> TsTable => _fakeTsTable;

        public FixtureForUpdatingBatchsOfRecords()
        {
            // ARRANGE
            var seed = Enumerable.Range(0, 600).Select(i => new TestingEntity("partitionKey", Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            var tableQuery = new TableQuery<TestingEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "partitionKey"));

            _fakeTsTable = new FakeTsTable<TestingEntity>(seed);
            var subject = new TsSet<TestingEntity>(_fakeTsTable);

            // ACT
            Result = subject.BatchUpdateAsync(
                tableQuery,
                entities =>
                {
                    entities.ForEach(e => e.MyProperty = "Test");
                }).Result;
        }
    }
}