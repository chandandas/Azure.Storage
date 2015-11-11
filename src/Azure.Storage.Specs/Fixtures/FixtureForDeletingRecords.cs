using System;
using Azure.Storage.Specs.TestClasses;
using Azure.Storage.Table;

namespace Azure.Storage.Specs.Fixtures
{
    public class FixtureForDeletingRecords : TsContextBaseFixture, IDisposable
    {
        public TestingEntity Entity { get; private set; }

        public FixtureForDeletingRecords()
        {
            // ARRANGE
            TableClient = GetTableClient();
            Table = CreateTable();
            Entity = CreateEntity();
            var subject = new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));

            // ACT
            subject.DeleteAsync(Entity).Wait();
        }

        public void Dispose()
        {
            DeleteTable();
        }
    }
}