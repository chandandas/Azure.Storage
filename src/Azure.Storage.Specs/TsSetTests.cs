namespace Azure.Storage.Specs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fixtures;
    using Microsoft.WindowsAzure.Storage.Table;
    using Table;
    using TestClasses;
    using Testing;
    using Xunit;

    [Collection("TsSet")]
    public class WhenAskedToFindOneRecord : IClassFixture<FixtureForFindingOneRecord>
    {
        private readonly TestingEntity _actualEntity;
        private readonly TestingEntity _expectedEntity;

        public WhenAskedToFindOneRecord(FixtureForFindingOneRecord fixture)
        {
            _actualEntity = fixture.ActualEntity;
            _expectedEntity = fixture.ExpectedEntity;
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
    }

    [Collection("TsSet")]
    public class WhenAskedToCreateARecord : IClassFixture<FixtureForCreatingRecords>
    {
        private readonly TestingEntity _actualEntity;
        private readonly TestingEntity _expectedEntity;

        public WhenAskedToCreateARecord(FixtureForCreatingRecords fixture)
        {
            _actualEntity = fixture.ActualEntity;
            _expectedEntity = fixture.ExpectedEntity;
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
    }

    [Collection("TsSet")]
    public class WhenAskedToUpdateARecord : IClassFixture<FixtureForUpdatingRecords>
    {
        private readonly TestingEntity _actualEntity;
        private readonly TestingEntity _expectedEntity;

        public WhenAskedToUpdateARecord(FixtureForUpdatingRecords fixture)
        {
            _actualEntity = fixture.ActualEntity;
            _expectedEntity = fixture.ExpectedEntity;
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
    }

    [Collection("TsSet")]
    public class WhenAskedToDeleteARecord : IClassFixture<FixtureForDeletingRecords>
    {
        private readonly FixtureForDeletingRecords _fixture;

        public WhenAskedToDeleteARecord(FixtureForDeletingRecords fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void ShouldDeleteTheEntity()
        {
            Assert.Null(_fixture.ActualEntity);
        }
    }

    [Collection("TsSet")]
    public class WhenAskedToQueryForMultipleRecords : IClassFixture<FixtureForQueryingMultipleRecords>
    {
        private readonly string _myPropertyValue;
        private readonly IEnumerable<TestingEntity> _results;

        public WhenAskedToQueryForMultipleRecords(FixtureForQueryingMultipleRecords fixture)
        {
            _results = fixture.Results;
            _myPropertyValue = fixture.PropertyValue;
        }

        [Fact]
        public void ShouldNotReturnNull()
        {
            Assert.NotNull(_results);
        }

        [Fact]
        public void ShouldReturnTheCorrectNumberOfResults()
        {
            Assert.Equal(50, _results.Count());
        }

        [Fact]
        public void ShouldReturnThePropertiesCorrectly()
        {
            Assert.True(_results.All(x => x.MyProperty == _myPropertyValue), "Has the correct perperty value.");
        }
    }

    [Collection("TsSet")]
    public class WhenAskedToProcessABatchOfRecords : IClassFixture<FixtureForProcessingABatchOfRecords>
    {
        private readonly List<string> _myProperties;

        private readonly BatchProcessResult _result;

        public WhenAskedToProcessABatchOfRecords(FixtureForProcessingABatchOfRecords fixture)
        {
            _result = fixture.Result;
            _myProperties = fixture.MyProperties;
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
    }

    [Collection("TsSet")]
    public class WhenAskedToCreateABatchOfRecords : IClassFixture<FixtureForCreatingBatchsOfRecords>
    {
        private readonly BatchAddResult _result;

        private readonly ITsTable<TestingEntity> _table;

        public WhenAskedToCreateABatchOfRecords(FixtureForCreatingBatchsOfRecords fixture)
        {
            _result = fixture.Result;
            _table = fixture.TsTable;
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
            var actual = await _table.ExecuteQuerySegmentedAsync(new TableQuery<TestingEntity>().Take(1000), null);
            Assert.Equal(600, actual.Results.Count);
        }
    }

    [Collection("TsSet")]
    public class WhenAskedToDeleteABatchOfRecords : IClassFixture<FixtureForDeletingBatchsOfRecords>
    {
        public WhenAskedToDeleteABatchOfRecords(FixtureForDeletingBatchsOfRecords fixture)
        {
            _result = fixture.Result;
            _table = fixture.TsTable;
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
        public void ShouldHaveDeletedItemsInTable()
        {
            var query = new TableQuery<TestingEntity>()
               .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "partitionKey"))
               .Take(1000);

            var actual = _table.ExecuteQuerySegmentedAsync(query, null).Result;

            Assert.Equal(0, actual.Results.Count);
        }

        private readonly BatchDeleteResult _result;
        private readonly ITsTable<TestingEntity> _table;
    }

    [Collection("TsSet")]
    public class WhenAskedToUpdateABatchOfRecords : IClassFixture<FixtureForUpdatingBatchsOfRecords>
    {
        public WhenAskedToUpdateABatchOfRecords(FixtureForUpdatingBatchsOfRecords fixture)
        {
            _result = fixture.Result;
            _table = fixture.TsTable;
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
        public void ShouldHaveUpdatedItemsInTable()
        {
            var query = new TableQuery<TestingEntity>()
               .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "partitionKey"));

            var actual = _table.ExecuteQuerySegmentedAsync(query, null).Result;

            Assert.True(actual.Results.All(e => e.MyProperty == "Test"));
        }

        private readonly BatchUpdateResult _result;
        private readonly ITsTable<TestingEntity> _table;
    }
}