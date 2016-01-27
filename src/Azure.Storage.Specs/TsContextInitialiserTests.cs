using Azure.Storage.Specs.Fixtures;
using Azure.Storage.Specs.TestClasses;
using Xunit;

namespace Azure.Storage.Specs
{
    public class WhenInitialisingAContext : IClassFixture<FixtureForInitialisingATsContext>
    {
        public WhenInitialisingAContext(FixtureForInitialisingATsContext fixture)
        {
            _context = fixture.Context;
        }

        [Fact]
        public void ShouldReturnTheInitialisedContext()
        {
            Assert.NotNull(_context);
            Assert.IsType<TestingContext>(_context);
        }

        [Fact]
        public void ShouldInitialiseThePropities()
        {
            Assert.NotNull(_context.TestingEntities);
        }

        private readonly TestingContext _context;
    }
}