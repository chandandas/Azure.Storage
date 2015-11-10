using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage.Table
{
    public class TsQueryResult<T>
    {
        public TsQueryResult(List<T> results, TableContinuationToken continuationToken)
        {
            Results = results;
            ContinuationToken = continuationToken;
        }

        public TableContinuationToken ContinuationToken { get; private set; }

        public List<T> Results { get; private set; }
    }
}