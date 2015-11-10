using Azure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage.Specs.TestClasses
{
    public class FindByMyPropertyQuery : TsQuery
    {
        public FindByMyPropertyQuery(string value)
            : base(TableQuery.GenerateFilterCondition("MyProperty", QueryComparisons.Equal, value))
        {
        }
    }
}