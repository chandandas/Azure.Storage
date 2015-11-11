using System;
using Azure.Storage.Specs.TestClasses;
using Azure.Storage.Table;

namespace Azure.Storage.Specs.Fixtures
{
    public class FixtureForFindingOneRecord : TsContextBaseFixture, IDisposable
    {
        public FixtureForFindingOneRecord()
        {
            TableClient = GetTableClient();
            Table = CreateTable();
            ExpectedEntity = CreateEntity();
            var subject = new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));

            // ACT
            ActualEntity = subject.FindAsync(ExpectedEntity.PartitionKey, ExpectedEntity.RowKey).Result;
        }

        public void Dispose()
        {
            DeleteTable();
        }

        public TestingEntity ActualEntity { get; set; }
        public TestingEntity ExpectedEntity { get; set; }
    }
}