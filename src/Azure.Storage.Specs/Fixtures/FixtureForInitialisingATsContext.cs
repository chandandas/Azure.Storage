using Azure.Storage.Specs.TestClasses;
using Azure.Storage.Table;

namespace Azure.Storage.Specs.Fixtures
{
    public class FixtureForInitialisingATsContext
    {
        public TestingContext Context { get; private set; }

        public FixtureForInitialisingATsContext()
        {
            // ARRANGE

            // ACT
            Context = TsContextInitialiser.Create<TestingContext>();
        }
    }
}