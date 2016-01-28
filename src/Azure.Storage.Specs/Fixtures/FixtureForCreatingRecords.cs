namespace Azure.Storage.Specs.Fixtures
{
    using System;
    using System.Collections.Generic;
    using Table;
    using TestClasses;
    using Testing;

    public class FixtureForCreatingRecords : TsContextBaseFixture
    {
        public FixtureForCreatingRecords()
        {
            ExpectedEntity = new TestingEntity();
            var fakeTsTable = new FakeTsTable<TestingEntity>(new List<TestingEntity>());
            var subject = new TsSet<TestingEntity>(fakeTsTable);

            // ACT
            subject.AddAsync(ExpectedEntity).Wait();

            ActualEntity = fakeTsTable.Entities[new Tuple<string, string>(ExpectedEntity.PartitionKey, ExpectedEntity.RowKey)];
        }

        public TestingEntity ActualEntity { get; private set; }
        public TestingEntity ExpectedEntity { get; }
    }
}