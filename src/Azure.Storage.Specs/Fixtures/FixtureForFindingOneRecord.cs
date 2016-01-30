namespace Azure.Storage.Specs.Fixtures
{
    using Table;
    using TestClasses;
    using Testing;

    public class FixtureForFindingOneRecord
    {
        public FixtureForFindingOneRecord()
        {
            ExpectedEntity = new TestingEntity();
            var subject = new TsSet<TestingEntity>(new FakeTsTable<TestingEntity>(new[] {ExpectedEntity}));

            // ACT
            ActualEntity = subject.FindAsync(ExpectedEntity.PartitionKey, ExpectedEntity.RowKey).Result;
        }

        public TestingEntity ActualEntity { get; private set; }
        public TestingEntity ExpectedEntity { get; }
    }
}