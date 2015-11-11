using System;
using Azure.Storage.Specs.TestClasses;
using Azure.Storage.Table;

namespace Azure.Storage.Specs.Fixtures
{
    public class FixtureForUpdatingRecords : TsContextBaseFixture, IDisposable
    {
        public TestingEntity ExpectedEntity { get; private set; }
        public TestingEntity ActualEntity { get; private set; }

        public FixtureForUpdatingRecords()
        {
            TableClient = GetTableClient();
            Table = CreateTable();
            var originalEntity = CreateEntity();
            ExpectedEntity = new TestingEntity
            {
                PartitionKey = originalEntity.PartitionKey,
                RowKey = originalEntity.RowKey,
                ETag = originalEntity.ETag,
                Timestamp = originalEntity.Timestamp,
                MyProperty = Guid.NewGuid().ToString("N")
            };

            var subject = new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));

            // ACT
            subject.UpdateAsync(ExpectedEntity).Wait();
            ActualEntity = GetEntity(ExpectedEntity.PartitionKey, ExpectedEntity.RowKey);
        }

        public void Dispose()
        {
            DeleteTable();
        }
    }
}