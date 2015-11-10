using Azure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage.Specs.TestClasses
{
    public class FindByPartitionKey : TsQuery
    {
        public FindByPartitionKey(string value) 
            : base(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, value))
        {
        }
    }
}