using System;
using Azure.Storage.Specs.TestClasses;
using Azure.Storage.Table;

namespace Azure.Storage.Specs.Fixtures
{
    using Testing;

    public class FixtureForUpdatingRecords : TsContextBaseFixture
    {
        public TestingEntity ExpectedEntity { get; private set; }
        public TestingEntity ActualEntity { get; private set; }

        public FixtureForUpdatingRecords()
        {
            var originalEntity = new TestingEntity();
            ExpectedEntity = new TestingEntity
            {
                PartitionKey = originalEntity.PartitionKey,
                RowKey = originalEntity.RowKey,
                ETag = originalEntity.ETag,
                Timestamp = originalEntity.Timestamp,
                MyProperty = Guid.NewGuid().ToString("N")
            };

            var fakeTsTable = new FakeTsTable<TestingEntity>(new[] { originalEntity });
            var subject = new TsSet<TestingEntity>(fakeTsTable);

            // ACT
            subject.UpdateAsync(ExpectedEntity).Wait();
            ActualEntity = fakeTsTable.Entities[new Tuple<string, string>(ExpectedEntity.PartitionKey, ExpectedEntity.RowKey)];
        }
    }
}