using System;
using Azure.Storage.Specs.TestClasses;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage.Specs.Fixtures
{
    public class TsContextBaseFixture
    {
        protected void CreateListOfEntities(int rowCount, string value = null)
        {
            for (int i = 0; i < rowCount; i++)
            {
                var entity = new TestingEntity(value ?? Guid.NewGuid().ToString("N"));
                TableOperation tableOperation = TableOperation.Insert(entity);
                Table.Execute(tableOperation);
            }
        }

        protected static CloudTableClient GetTableClient()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            return tableClient;
        }

        protected TestingEntity CreateEntity()
        {
            var testingEntity = new TestingEntity();
            Table = GetTable();

            TableOperation tableOperation = TableOperation.Insert(testingEntity);
            Table.Execute(tableOperation);
            return testingEntity;
        }

        public TestingEntity GetEntity(string partitionKey, string rowKey)
        {
            var table = CreateTable();
            TableOperation tableOperation = TableOperation.Retrieve<TestingEntity>(partitionKey, rowKey);
            return table.Execute(tableOperation).Result as TestingEntity;
        }

        protected CloudTable CreateTable()
        {
            var table = GetTable();
            table.CreateIfNotExists();
            return table;
        }

        protected void DeleteTable()
        {
            CloudTable table = GetTable();
            table.DeleteIfExists();
        }

        public CloudTable GetTable()
        {
            return TableClient.GetTableReference("TestingEntities");
        }

        protected CloudTable Table;
        protected CloudTableClient TableClient;
    }
}