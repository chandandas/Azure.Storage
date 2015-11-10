using System.Collections.Generic;
using Azure.Storage.Table;

namespace Azure.Storage.Specs.TestClasses
{
    public class TestingContext : TsContext
    {
        public TestingContext()
            : base("defaultConnection")
        {
        }

        public TsSet<TestingEntity> TestingEntities { get; set; }

        public List<string> DontUseThis { get; set; }
    }
}