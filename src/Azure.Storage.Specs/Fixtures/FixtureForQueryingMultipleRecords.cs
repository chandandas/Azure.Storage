using System;
using System.Collections.Generic;
using Azure.Storage.Specs.TestClasses;
using Azure.Storage.Table;

namespace Azure.Storage.Specs.Fixtures
{
    public class FixtureForQueryingMultipleRecords : TsContextBaseFixture, IDisposable
    {
        public IEnumerable<TestingEntity> Results;
        public readonly string PropertyValue;

        public FixtureForQueryingMultipleRecords()
        {
            // ARRANGE
            TableClient = GetTableClient();
            Table = CreateTable();

            PropertyValue = Guid.NewGuid().ToString("N");
            CreateListOfEntities(50, PropertyValue);
            CreateListOfEntities(150);

            var subject = new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));

            Results = subject.QueryAsync(new FindByMyPropertyQuery(PropertyValue)).Result;
        }

        public void Dispose()
        {
            DeleteTable();
        }
    }
}