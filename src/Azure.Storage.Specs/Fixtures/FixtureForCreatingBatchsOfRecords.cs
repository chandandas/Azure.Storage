using System;
using System.Collections.Generic;
using Azure.Storage.Specs.TestClasses;
using Azure.Storage.Table;

namespace Azure.Storage.Specs.Fixtures
{
    public class FixtureForCreatingBatchsOfRecords : TsContextBaseFixture, IDisposable
    {
        public BatchAddResult Result { get; private set; }

        public FixtureForCreatingBatchsOfRecords()
        {
            // ARRANGE
            TableClient = GetTableClient();
            Table = CreateTable();

            var testingEntites = new List<TestingEntity>();

            for (var i = 0; i < 600; i++)
            {
                testingEntites.Add(
                    new TestingEntity("partitionKey", Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            }

            var subject = new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));

            // ACT
            Result = subject.BatchAddAsync(testingEntites).Result;
        }

        public void Dispose()
        {
            DeleteTable();
        }
    }
}