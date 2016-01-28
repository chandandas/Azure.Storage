namespace Azure.Storage.Specs.Fixtures
{
    using System;
    using System.Collections.Generic;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using TestClasses;

    public class TsContextBaseFixture
    {
        protected CloudTable Table;
        protected CloudTableClient TableClient;

        protected IEnumerable<TestingEntity> CreateListOfEntities(int rowCount, string value = null)
        {
            for (var i = 0; i < rowCount; i++)
            {
                yield return new TestingEntity(value ?? Guid.NewGuid().ToString("N"));
            }
        }

        protected static CloudTableClient GetTableClient()
        {
            var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var tableClient = storageAccount.CreateCloudTableClient();
            return tableClient;
        }

        protected CloudTable CreateTable()
        {
            var table = GetTable();
            table.CreateIfNotExists();
            return table;
        }

        protected void DeleteTable()
        {
            var table = GetTable();
            table.DeleteIfExists();
        }

        public CloudTable GetTable()
        {
            return TableClient.GetTableReference("TestingEntities");
        }
    }
}