namespace Azure.Storage.Specs.Fixtures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Table;
    using TestClasses;
    using Testing;

    public class FixtureForProcessingABatchOfRecords
    {
        public FixtureForProcessingABatchOfRecords()
        {
            // ARRANGE
            var seed = Enumerable.Range(0, 600)
                    .Select(i => new TestingEntity("partitionKey", Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            var subject = new TsSet<TestingEntity>(new FakeTsTable<TestingEntity>(seed));

            // ACT
            MyProperties = new List<string>();
            Result = subject.BatchProcessAsync(
                new FindByPartitionKey("partitionKey"),
                entities =>
                {
                    MyProperties.AddRange(entities.Select(f => f.MyProperty));
                    return Task.Delay(0);
                }).Result;
        }

        public List<string> MyProperties { get; }
        public BatchProcessResult Result { get; private set; }
    }
}