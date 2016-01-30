using System;
using System.Collections.Generic;
using Azure.Storage.Specs.TestClasses;
using Azure.Storage.Table;

namespace Azure.Storage.Specs.Fixtures
{
    using Testing;

    public class FixtureForQueryingMultipleRecords
    {
        public IEnumerable<TestingEntity> Results;
        public readonly string PropertyValue;

        public FixtureForQueryingMultipleRecords()
        {
            // ARRANGE
            var testingEntities = new List<TestingEntity>();

            PropertyValue = Guid.NewGuid().ToString("N");
            testingEntities.AddRange(CreateListOfEntities(50, PropertyValue));
            testingEntities.AddRange(CreateListOfEntities(150));

            var subject = new TsSet<TestingEntity>(new FakeTsTable<TestingEntity>(testingEntities));

            Results = subject.QueryAsync(new FindByMyPropertyQuery(PropertyValue)).Result;
        }

        private IEnumerable<TestingEntity> CreateListOfEntities(int rowCount, string value = null)
        {
            for (var i = 0; i < rowCount; i++)
            {
                yield return new TestingEntity(value ?? Guid.NewGuid().ToString("N"));
            }
        }
    }
}