using Azure.Storage.Specs.Fixtures;
using Azure.Storage.Specs.TestClasses;
using Microsoft.WindowsAzure.Storage.Table;
using Xunit;

namespace Azure.Storage.Specs
{
    public class WhenInitialisingAContext : IClassFixture<FixtureForInitialisingATsContext>
    {
        public WhenInitialisingAContext(FixtureForInitialisingATsContext fixture)
        {
            _table = fixture.GetTable();
            _context = fixture.Context;
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
        
        private readonly CloudTable _table;
        private readonly TestingContext _context;
    }
}