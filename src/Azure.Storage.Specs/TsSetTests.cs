using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Specs.TestClasses;
using Azure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Xunit;

namespace Azure.Storage.Specs
{
    public class TsSetTestBase
    {
        protected void CreateListOfEntities(string value, int rowCount)
        {
            for (int i = 0; i < rowCount; i++)
            {
                var entity = new TestingEntity(value);
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

        protected TestingEntity GetEntity(string partitionKey, string rowKey)
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

        private CloudTable GetTable()
        {
            return TableClient.GetTableReference("TestingEntities");
        }

        protected CloudTable Table;
        protected CloudTableClient TableClient;
    }

    public class WhenConstructingATsSet : TsSetTestBase, IDisposable
    {
        public WhenConstructingATsSet()
        {
            // ARRANGE
            TableClient = GetTableClient();
            Table = TableClient.GetTableReference("TestingEntities");

            // ACT
            new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));
        }

        [Fact]
        public void ShouldCreateTheTable()
        {
            Assert.True(Table.Exists(), "Table has been created.");
        }

        [Fact]
        public void ShouldHaveTheCorrectName()
        {
            Assert.Equal("TestingEntities", Table.Name);
        }

        public void Dispose()
        {
            DeleteTable();
        }
    }

    public class WhenAskedToFindOneRecord : TsSetTestBase, IDisposable
    {
        public WhenAskedToFindOneRecord()
        {
            TableClient = GetTableClient();
            Table = CreateTable();
            _expectedEntity = CreateEntity();
            var subject = new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));

            // ACT
            _actualEntity = subject.FindAsync(_expectedEntity.PartitionKey, _expectedEntity.RowKey).Result;
        }
        
        [Fact]
        public void ShouldReturnTheEntity()
        {
            Assert.NotNull(_actualEntity);
        }

        [Fact]
        public void ShouldReturnThePropertiesCorrectly()
        {
            Assert.Equal(_expectedEntity.PartitionKey, _actualEntity.PartitionKey);
            Assert.Equal(_expectedEntity.RowKey, _actualEntity.RowKey);
            Assert.Equal(_expectedEntity.MyProperty, _actualEntity.MyProperty);
        }

        public void Dispose()
        {
            DeleteTable();
        }

        private readonly TestingEntity _actualEntity;
        private readonly TestingEntity _expectedEntity;
    }

    public class WhenAskedToCreateARecord : TsSetTestBase, IDisposable
    {
        public WhenAskedToCreateARecord()
        {
            // ARRANGE
            TableClient = GetTableClient();
            Table = CreateTable();
            _expectedEntity = new TestingEntity();
            var subject = new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));

            // ACT
            var task = subject.AddAsync(_expectedEntity);
            task.Wait();
            _actualEntity = GetEntity(_expectedEntity.PartitionKey, _expectedEntity.RowKey);
        }

        [Fact]
        public void ShouldSaveTheEntity()
        {
            Assert.NotNull(_actualEntity);
        }

        [Fact]
        public void ShouldSaveThePropertiesCorrectly()
        {
            Assert.Equal(_expectedEntity.PartitionKey, _actualEntity.PartitionKey);
            Assert.Equal(_expectedEntity.RowKey, _actualEntity.RowKey);
            Assert.Equal(_expectedEntity.MyProperty, _actualEntity.MyProperty);
        }

        public void Dispose()
        {
            DeleteTable();
        }

        private readonly TestingEntity _actualEntity;
        private readonly TestingEntity _expectedEntity;
    }

    public class WhenAskedToCreateABatchOfRecords : TsSetTestBase, IDisposable
    {
        public WhenAskedToCreateABatchOfRecords()
        {
            // ARRANGE
            TableClient = GetTableClient();
            Table = CreateTable();

            var testingEntites = new List<TestingEntity>();

            for (var i = 0; i < 600; i++)
            {
                testingEntites.Add(
                    new TestingEntity("partitionKey", Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            }

            var subject = new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));

            // ACT
            _result = subject.BatchAddAsync(testingEntites).Result;
        }

        [Fact]
        public void ShouldHaveRanCreateBatches()
        {
            Assert.Equal(6, _result.Batches);
        }

        [Fact]
        public void ShouldHaveCreatedEntities()
        {
            Assert.Equal(600, _result.EntitiesCreated);
        }

        [Fact]
        public async Task ShouldHaveCreatedRecordsInTable()
        {
            var table = CreateTable();
            var query = new TableQuery<TestingEntity>();
            var actual = await table.ExecuteQuerySegmentedAsync(query, null);

            Assert.Equal(600, actual.Results.Count);
        }

        public void Dispose()
        {
            DeleteTable();
        }

        private readonly BatchAddResult _result;
    }

    public class WhenAskedToProcessABatchOfRecords : TsSetTestBase, IDisposable
    {
        public WhenAskedToProcessABatchOfRecords()
        {
            // ARRANGE
            TableClient = GetTableClient();
            Table = CreateTable();

            var tasks = Enumerable.Range(0, 600).Select(i =>
            {
                var op = TableOperation.Insert(new TestingEntity("partitionKey", Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
                return Table.ExecuteAsync(op);
            });

            Task.WhenAll(tasks).Wait();

            _myProperties = new List<string>();

            var subject = new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));

            // ACT
            _result = subject.BatchProcessAsync(
                new FindByPartitionKey("partitionKey"),
                entities =>
                {
                    _myProperties.AddRange(entities.Select(f => f.MyProperty));
                    return Task.Delay(0);
                }).Result;
        }

        [Fact]
        public void ShouldReturnTheCorrectBatchesResult()
        {
            Assert.Equal(2, _result.Batches);
        }

        [Fact]
        public void ShouldReturnTheCorrectEntitiesProcessedResult()
        {
            Assert.Equal(600, _result.EntitiesProcessed);
        }

        [Fact]
        public void ShouldHaveProcessedItems()
        {
            Assert.Equal(600, _myProperties.Count);
        }

        public void Dispose()
        {
            DeleteTable();
        }

        private readonly BatchProcessResult _result;
        private readonly List<string> _myProperties;
    }

    public class WhenAskedToDeleteABatchOfRecords : TsSetTestBase, IDisposable
    {
        public WhenAskedToDeleteABatchOfRecords()
        {
            // ARRANGE
            TableClient = GetTableClient();
            Table = CreateTable();

            var tasks = Enumerable.Range(0, 600).Select(i =>
            {
                var op = TableOperation.Insert(new TestingEntity("partitionKey", Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
                return Table.ExecuteAsync(op);
            });

            Task.WhenAll(tasks).Wait();

            var subject = new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));

            // ACT
            _result = subject.BatchDeleteAsync(new FindByPartitionKey("partitionKey")).Result;
        }

        [Fact]
        public void ShouldReturnTheCorrectBatchesResult()
        {
            Assert.Equal(2, _result.Batches);
        }

        [Fact]
        public void ShouldReturnTheCorrectDeleteBatchesResult()
        {
            Assert.Equal(6, _result.DeleteBatches);
        }

        [Fact]
        public void ShouldReturnTheCorrectEntitesDeletedResult()
        {
            Assert.Equal(600, _result.EntitiesDeleted);
        }

        [Fact]
        public void ShouldNotHaveAnyPostsInNewsFeed()
        {
            var query = new TableQuery<TestingEntity>()
               .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "partitionKey"));

            var actual = Table.ExecuteQuerySegmentedAsync(query, null).Result;

            Assert.Equal(0, actual.Results.Count);
        }

        public void Dispose()
        {
            DeleteTable();
        }

        private readonly BatchDeleteResult _result;
    }

    public class WhenAskedToUpdateABatchOfRecords : TsSetTestBase, IDisposable
    {
        public WhenAskedToUpdateABatchOfRecords()
        {
            // ARRANGE
            TableClient = GetTableClient();
            Table = CreateTable();

            var tasks = Enumerable.Range(0, 600).Select(i =>
            {
                var op = TableOperation.Insert(new TestingEntity("partitionKey", Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
                return Table.ExecuteAsync(op);
            });

            Task.WhenAll(tasks).Wait();

            var tableQuery = new TableQuery<TestingEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "partitionKey"));

            var subject = new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));

            // ACT
            _result = subject.BatchUpdateAsync(
                tableQuery,
                entities =>
                {
                    entities.ForEach(e => e.MyProperty = "Test");
                }).Result;
        }

        [Fact]
        public void ShouldReturnTheCorrectBatchesResult()
        {
            Assert.Equal(2, _result.Batches);
        }

        [Fact]
        public void ShouldReturnTheCorrectUpdateBatchesResult()
        {
            Assert.Equal(6, _result.UpdatedBatches);
        }

        [Fact]
        public void ShouldReturnTheCorrectEntitesUpdatedResult()
        {
            Assert.Equal(600, _result.EntitiesUpdated);
        }

        [Fact]
        public void ShouldNotHaveAnyPostsInNewsFeed()
        {
            var query = new TableQuery<TestingEntity>()
               .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "partitionKey"));

            var actual = Table.ExecuteQuerySegmentedAsync(query, null).Result;

            Assert.True(actual.Results.All(e => e.MyProperty == "Test"));
        }

        public void Dispose()
        {
            DeleteTable();
        }

        private readonly BatchUpdateResult _result;
    }

    public class WhenAskedToUpdateARecord : TsSetTestBase, IDisposable
    {
        public WhenAskedToUpdateARecord()
        {
            // ARRANGE
            TableClient = GetTableClient();
            Table = CreateTable();
            var originalEntity = CreateEntity();
            _expectedEntity = new TestingEntity
            {
                PartitionKey = originalEntity.PartitionKey,
                RowKey = originalEntity.RowKey,
                ETag = originalEntity.ETag,
                Timestamp = originalEntity.Timestamp,
                MyProperty = Guid.NewGuid().ToString("N")
            };
            var subject = new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));

            // ACT
            subject.UpdateAsync(_expectedEntity).Wait();
            _actualEntity = GetEntity(_expectedEntity.PartitionKey, _expectedEntity.RowKey);
        }

        [Fact]
        public void ShouldSaveTheEntity()
        {
            Assert.NotNull(_actualEntity);
        }

        [Fact]
        public void ShouldSaveThePropertiesCorrectly()
        {
            Assert.Equal(_expectedEntity.PartitionKey, _actualEntity.PartitionKey);
            Assert.Equal(_expectedEntity.RowKey, _actualEntity.RowKey);
            Assert.Equal(_expectedEntity.MyProperty, _actualEntity.MyProperty);
        }

        public void Dispose()
        {
            DeleteTable();
        }

        private readonly TestingEntity _actualEntity;
        private readonly TestingEntity _expectedEntity;
    }

    public class WhenAskedToDeleteARecord : TsSetTestBase, IDisposable
    {
        public WhenAskedToDeleteARecord()
        {
            // ARRANGE
            TableClient = GetTableClient();
            Table = CreateTable();
            _actualEntity = CreateEntity();
            var subject = new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));

            // ACT
            subject.DeleteAsync(_actualEntity).Wait();
        }

        [Fact]
        public void ShouldDeleteTheEntity()
        {
            _expectedEntity = GetEntity(_actualEntity.PartitionKey, _actualEntity.RowKey);
            Assert.Null(_expectedEntity);
        }

        public void Dispose()
        {
            DeleteTable();
        }

        private readonly TestingEntity _actualEntity;
        private TestingEntity _expectedEntity;
    }
    
    public class WhenAskedToQueryForMultipleRecords : TsSetTestBase, IDisposable
    {
        public WhenAskedToQueryForMultipleRecords()
        {
            // ARRANGE
            TableClient = GetTableClient();
            Table = CreateTable();
            _myPropertyValue = Guid.NewGuid().ToString("N");
            CreateListOfEntities(_myPropertyValue, 600);
            CreateListOfEntities(Guid.NewGuid().ToString("N"), 5);

            var query = new FindByMyPropertyQuery(_myPropertyValue);

            var subject = new TsSet<TestingEntity>(new TsTable<TestingEntity>(Table));
            _results = subject.QueryAsync(query).Result;
        }

        [Fact]
        public void ShouldNotReturnNull()
        {
            Assert.NotNull(_results);
        }

        [Fact]
        public void ShouldReturnTheCorrectNumberOfResults()
        {
            Assert.Equal(600, _results.Count());
        }

        [Fact]
        public void ShouldReturnThePropertiesCorrectly()
        {
            Assert.True(_results.All(x => x.MyProperty == _myPropertyValue), "Has the correct perperty value.");
        }

        public void Dispose()
        {
            DeleteTable();
        }

        private readonly string _myPropertyValue;
        private readonly IEnumerable<TestingEntity> _results;
    }
}