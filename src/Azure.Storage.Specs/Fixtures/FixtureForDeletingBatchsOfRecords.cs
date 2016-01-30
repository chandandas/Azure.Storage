namespace Azure.Storage.Specs.Fixtures
{
    using System;
    using System.Linq;
    using Table;
    using TestClasses;
    using Testing;

    public class FixtureForDeletingBatchsOfRecords
    {
        private FakeTsTable<TestingEntity> _fakeTsTable;

        public FixtureForDeletingBatchsOfRecords()
        {
            // ARRANGE
            var seed = Enumerable.Range(0, 600)
                .Select(i => new TestingEntity("partitionKey", Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            _fakeTsTable = new FakeTsTable<TestingEntity>(seed);
            var subject = new TsSet<TestingEntity>(_fakeTsTable);

            // ACT
            Result = subject.BatchDeleteAsync(new FindByPartitionKey("partitionKey")).Result;
        }

        public BatchDeleteResult Result { get; private set; }

        public FakeTsTable<TestingEntity> TsTable => _fakeTsTable;
    }
}