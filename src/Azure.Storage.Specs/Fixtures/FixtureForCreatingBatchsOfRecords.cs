namespace Azure.Storage.Specs.Fixtures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Table;
    using TestClasses;
    using Testing;

    public class FixtureForCreatingBatchsOfRecords
    {
        private readonly FakeTsTable<TestingEntity> _fakeTsTable;

        public FixtureForCreatingBatchsOfRecords()
        {
            // ARRANGE
            var testingEntites = Enumerable.Range(0, 600)
                .Select(i => new TestingEntity("partitionKey", Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            _fakeTsTable = new FakeTsTable<TestingEntity>(new List<TestingEntity>());
            var subject = new TsSet<TestingEntity>(_fakeTsTable);

            // ACT
            Result = subject.BatchAddAsync(testingEntites.ToList()).Result;
        }

        public BatchAddResult Result { get; private set; }

        public FakeTsTable<TestingEntity> TsTable => _fakeTsTable;
    }
}