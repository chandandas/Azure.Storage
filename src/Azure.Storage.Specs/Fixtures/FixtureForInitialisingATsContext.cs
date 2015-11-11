using System;
using Azure.Storage.Specs.TestClasses;
using Azure.Storage.Table;

namespace Azure.Storage.Specs.Fixtures
{
    public class FixtureForInitialisingATsContext : TsContextBaseFixture, IDisposable
    {
        public TestingContext Context { get; private set; }

        public FixtureForInitialisingATsContext()
        {
            // ARRANGE
            TableClient = GetTableClient();
            Table = GetTable();

            // ACT
            Context = TsContextInitialiser.Create<TestingContext>();
        }

        public void Dispose()
        {
            DeleteTable();
        }
    }
}