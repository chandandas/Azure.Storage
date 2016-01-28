namespace Azure.Storage.Specs.Fixtures
{
    using System;
    using Table;
    using TestClasses;
    using Testing;

    public class FixtureForDeletingRecords : TsContextBaseFixture
    {
        public FixtureForDeletingRecords()
        {
            // ARRANGE
            Entity = new TestingEntity();
            var fakeTsTable = new FakeTsTable<TestingEntity>(new[] {Entity});
            var subject = new TsSet<TestingEntity>(fakeTsTable);

            // ACT
            subject.DeleteAsync(Entity).Wait();

            try
            {
                ActualEntity = fakeTsTable.Entities[new Tuple<string, string>(Entity.PartitionKey, Entity.RowKey)];
            }
            catch (Exception)
            {
                ActualEntity = null;
            }
        }

        public TestingEntity Entity { get; }
        public TestingEntity ActualEntity { get; }
    }
}