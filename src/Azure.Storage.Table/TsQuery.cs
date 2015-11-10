using System;

namespace Azure.Storage.Table
{
    public abstract class TsQuery
    {
        protected TsQuery(string query)
        {
            if(query == null)
                throw new ArgumentNullException("query");

            Query = query;
        }

        public string Query { get; private set; }
    }
}