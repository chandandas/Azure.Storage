using System.Collections.Generic;
using System.Linq;
using Azure.Storage.Specs.Fixtures;
using Azure.Storage.Specs.TestClasses;
using Xunit;

namespace Azure.Storage.Specs
{
    [Collection("TsSet")]
    public class WhenAskedToFindOneRecord : IClassFixture<FixtureForFindingOneRecord>
    {
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

        private readonly TestingEntity _actualEntity;
        private readonly TestingEntity _expectedEntity;
    }

    [Collection("TsSet")]
    public class WhenAskedToCreateARecord : IClassFixture<FixtureForCreatingRecords>
    {
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

        private readonly TestingEntity _actualEntity;
        private readonly TestingEntity _expectedEntity;
    }

    [Collection("TsSet")]
    public class WhenAskedToUpdateARecord : IClassFixture<FixtureForUpdatingRecords>
    {
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

        private readonly TestingEntity _actualEntity;
        private readonly TestingEntity _expectedEntity;
    }

    [Collection("TsSet")]
    public class WhenAskedToDeleteARecord : IClassFixture<FixtureForDeletingRecords>
    {
        public WhenAskedToDeleteARecord(FixtureForDeletingRecords fixture)
        {
            var entity = fixture.Entity;
            _actualEntity = fixture.GetEntity(entity.PartitionKey, entity.RowKey);
        }

        [Fact]
        public void ShouldDeleteTheEntity()
        {
            Assert.Null(_actualEntity);
        }

        private readonly TestingEntity _actualEntity;
    }

    [Collection("TsSet")]
    public class WhenAskedToQueryForMultipleRecords : IClassFixture<FixtureForQueryingMultipleRecords>
    {
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

        private readonly string _myPropertyValue;
        private readonly IEnumerable<TestingEntity> _results;
    }

    //[Collection("TsSet")]
    //public class WhenAskedToProcessABatchOfRecords : IClassFixture<FixtureForProcessingABatchOfRecords>
    //{
    //    public WhenAskedToProcessABatchOfRecords(FixtureForProcessingABatchOfRecords fixture)
    //    {
    //        _result = fixture.Result;
    //        _myProperties = fixture.MyProperties;
    //    }

    //    [Fact]
    //    public void ShouldReturnTheCorrectBatchesResult()
    //    {
    //        Assert.Equal(2, _result.Batches);
    //    }

    //    [Fact]
    //    public void ShouldReturnTheCorrectEntitiesProcessedResult()
    //    {
    //        Assert.Equal(600, _result.EntitiesProcessed);
    //    }

    //    [Fact]
    //    public void ShouldHaveProcessedItems()
    //    {
    //        Assert.Equal(600, _myProperties.Count);
    //    }

    //    private readonly BatchProcessResult _result;
    //    private readonly List<string> _myProperties;
    //}

    //[Collection("TsSet")]
    //public class WhenAskedToCreateABatchOfRecords : IClassFixture<FixtureForCreatingBatchsOfRecords>
    //{
    //    public WhenAskedToCreateABatchOfRecords(FixtureForCreatingBatchsOfRecords fixture)
    //    {
    //        _result = fixture.Result;
    //        _table = fixture.GetTable();
    //    }

    //    [Fact]
    //    public void ShouldHaveRanCreateBatches()
    //    {
    //        Assert.Equal(6, _result.Batches);
    //    }

    //    [Fact]
    //    public void ShouldHaveCreatedEntities()
    //    {
    //        Assert.Equal(600, _result.EntitiesCreated);
    //    }

    //    [Fact]
    //    public async Task ShouldHaveCreatedRecordsInTable()
    //    {
    //        var actual = await _table.ExecuteQuerySegmentedAsync(new TableQuery<TestingEntity>(), null);
    //        Assert.Equal(600, actual.Results.Count);
    //    }

    //    private readonly CloudTable _table;
    //    private readonly BatchAddResult _result;
    //}

    //[Collection("TsSet")]
    //public class WhenAskedToDeleteABatchOfRecords : IClassFixture<FixtureForDeletingBatchsOfRecords>
    //{
    //    public WhenAskedToDeleteABatchOfRecords(FixtureForDeletingBatchsOfRecords fixture)
    //    {
    //        _result = fixture.Result;
    //        _table = fixture.GetTable();
    //    }

    //    [Fact]
    //    public void ShouldReturnTheCorrectBatchesResult()
    //    {
    //        Assert.Equal(2, _result.Batches);
    //    }

    //    [Fact]
    //    public void ShouldReturnTheCorrectDeleteBatchesResult()
    //    {
    //        Assert.Equal(6, _result.DeleteBatches);
    //    }

    //    [Fact]
    //    public void ShouldReturnTheCorrectEntitesDeletedResult()
    //    {
    //        Assert.Equal(600, _result.EntitiesDeleted);
    //    }

    //    [Fact]
    //    public void ShouldNotHaveAnyPostsInNewsFeed()
    //    {
    //        var query = new TableQuery<TestingEntity>()
    //           .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "partitionKey"));

    //        var actual = _table.ExecuteQuerySegmentedAsync(query, null).Result;

    //        Assert.Equal(0, actual.Results.Count);
    //    }

    //    private readonly BatchDeleteResult _result;
    //    private CloudTable _table;
    //}

    //[Collection("TsSet")]
    //public class WhenAskedToUpdateABatchOfRecords : IClassFixture<FixtureForUpdatingBatchsOfRecords>
    //{
    //    public WhenAskedToUpdateABatchOfRecords(FixtureForUpdatingBatchsOfRecords fixture)
    //    {
    //        _result = fixture.Result;
    //        _table = fixture.GetTable();
    //    }

    //    [Fact]
    //    public void ShouldReturnTheCorrectBatchesResult()
    //    {
    //        Assert.Equal(2, _result.Batches);
    //    }

    //    [Fact]
    //    public void ShouldReturnTheCorrectUpdateBatchesResult()
    //    {
    //        Assert.Equal(6, _result.UpdatedBatches);
    //    }

    //    [Fact]
    //    public void ShouldReturnTheCorrectEntitesUpdatedResult()
    //    {
    //        Assert.Equal(600, _result.EntitiesUpdated);
    //    }

    //    [Fact]
    //    public void ShouldNotHaveAnyPostsInNewsFeed()
    //    {
    //        var query = new TableQuery<TestingEntity>()
    //           .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "partitionKey"));

    //        var actual = _table.ExecuteQuerySegmentedAsync(query, null).Result;

    //        Assert.True(actual.Results.All(e => e.MyProperty == "Test"));
    //    }

    //    private readonly CloudTable _table;
    //    private readonly BatchUpdateResult _result;
    //}
}