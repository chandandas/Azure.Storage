using System;
using Azure.Storage.Specs.TestClasses;
using Azure.Storage.Table;

namespace Azure.Storage.Specs.Fixtures
{
    public class FixtureForConstructingTsSets : TsContextBaseFixture, IDisposable
    {
        public FixtureForConstructingTsSets()
        {
            // ARRANGE
            TableClient = GetTableClient();
            Table = TableClient.GetTableReference("TestingEntities");

            // ACT
            new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));
        }

        public void Dispose()
        {
            DeleteTable();
        }
    }
}