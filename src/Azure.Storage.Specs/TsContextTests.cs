using Azure.Storage.Table;
using Xunit;

namespace Azure.Storage.Specs
{
    public class WhenConstructingATsContextWithAConnectionString
    {
        public WhenConstructingATsContextWithAConnectionString()
        {
            // ACT
            _tsContext = new TsContext("UseDevelopmentStorage=true");
            _tsContext.Initialise();
        }

        [Fact]
        public void ShouldCreateATableClient()
        {
            Assert.NotNull(_tsContext.TableClient);
        }

        private readonly TsContext _tsContext;
    }

    public class WhenConstructingATsContextWithAConnectionStringName
    {
        public WhenConstructingATsContextWithAConnectionStringName()
        {
            // ACT
            _tsContext = new TsContext("DefaultConnection");
            _tsContext.Initialise();
        }

        [Fact]
        public void ShouldCreateATableClient()
        {
            Assert.NotNull(_tsContext.TableClient);
        }

        private readonly TsContext _tsContext;
    }
}