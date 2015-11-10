using System;
using Azure.Storage.Specs.TestClasses;
using Azure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Xunit;

namespace Azure.Storage.Specs
{
    public class WhenInitialisingAContext : IDisposable
    {
        public WhenInitialisingAContext()
        {
            // ARRANGE
            _tableClient = GetTableClient();
            _table = GetTable();

            // ACT
            _context = TsContextInitialiser.Create<TestingContext>();
        }

        [Fact]
        public void ShouldCreateATable()
        {
            Assert.True(_table.Exists(), "Table has been created.");
        }

        [Fact]
        public void ShouldReturnTheInitialisedContext()
        {
            Assert.NotNull(_context);
            Assert.IsType<TestingContext>(_context);
        }

        private static CloudTableClient GetTableClient()
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            return cloudStorageAccount.CreateCloudTableClient();
        }
        
        private CloudTable GetTable()
        {
            return _tableClient.GetTableReference("TestingEntities");
        }

        public void Dispose()
        {
            _table.DeleteIfExists();
        }

        private readonly CloudTable _table;
        private readonly TestingContext _context;
        private readonly CloudTableClient _tableClient;
    }
}