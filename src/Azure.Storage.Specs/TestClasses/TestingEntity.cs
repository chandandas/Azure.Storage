using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage.Specs.TestClasses
{
    public class TestingEntity : TableEntity
    {
        public TestingEntity()
            : this(Guid.NewGuid().ToString("N"))
        {
        }

        public TestingEntity(string value)
            : this(Guid.NewGuid().ToString("N"), DateTime.UtcNow.ToString("u"), value)
        {
        }

        public TestingEntity(string partitionKey, string rowKey, string value)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            MyProperty = value;
        }

        public string MyProperty { get; set; }
    }
}