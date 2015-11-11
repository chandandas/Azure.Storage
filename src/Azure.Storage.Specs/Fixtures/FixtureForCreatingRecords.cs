using System;
using Azure.Storage.Specs.TestClasses;
using Azure.Storage.Table;

namespace Azure.Storage.Specs.Fixtures
{
    public class FixtureForCreatingRecords : TsContextBaseFixture, IDisposable
    {
        public FixtureForCreatingRecords()
        {
            TableClient = GetTableClient();
            Table = CreateTable();

            ExpectedEntity = new TestingEntity();
            var subject = new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));

            // ACT
            subject.AddAsync(ExpectedEntity).Wait();

            ActualEntity = GetEntity(ExpectedEntity.PartitionKey, ExpectedEntity.RowKey);
        }

        public void Dispose()
        {
            DeleteTable();
        }

        public TestingEntity ActualEntity { get; private set; }
        public TestingEntity ExpectedEntity { get; private set; }
    }
}